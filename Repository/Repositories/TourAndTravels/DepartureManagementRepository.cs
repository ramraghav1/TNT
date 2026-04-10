using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.TourAndTravels;
using static Repository.DataModels.TourAndTravels.DepartureManagementDTO;

namespace Repository.Repositories.TourAndTravels
{
    public class DepartureManagementRepository : IDepartureManagementRepository
    {
        private readonly IDbConnection _dbConnection;

        public DepartureManagementRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // GET UNASSIGNED BOOKINGS (departure_id IS NULL)
        // ============================================================
        public List<UnassignedBookingItem> GetUnassignedBookings()
        {
            const string sql = @"
                SELECT
                    ii.id                          AS InstanceId,
                    ii.booking_reference           AS BookingReference,
                    ii.template_itinerary_id       AS ItineraryId,
                    i.title                        AS ItineraryTitle,
                    ii.start_date                  AS StartDate,
                    ii.end_date                    AS EndDate,
                    ii.total_person                AS TotalPerson,
                    ii.status                      AS Status,
                    ii.payment_status              AS PaymentStatus,
                    ii.total_amount                AS TotalAmount,
                    ii.created_at                  AS CreatedAt,
                    t.full_name                    AS LeadTravelerName,
                    t.email                        AS LeadTravelerEmail,
                    t.contact_number               AS LeadTravelerPhone
                FROM itinerary_instances ii
                JOIN itineraries i ON i.id = ii.template_itinerary_id
                LEFT JOIN LATERAL (
                    SELECT full_name, email, contact_number
                    FROM travelers
                    WHERE itinerary_instance_id = ii.id
                    ORDER BY id
                    LIMIT 1
                ) t ON true
                WHERE ii.departure_id IS NULL
                ORDER BY i.title, ii.start_date;";

            return _dbConnection.Query<UnassignedBookingItem>(sql).ToList();
        }

        // ============================================================
        // GET SUGGESTED DEPARTURES
        // ============================================================
        public List<SuggestedDepartureItem> GetSuggestedDepartures(long itineraryId, int requiredSeats)
        {
            const string sql = @"
                SELECT
                    pd.id                              AS Id,
                    i.title                            AS ItineraryTitle,
                    pd.departure_date                  AS DepartureDate,
                    pd.end_date                        AS EndDate,
                    pd.available_seats                 AS AvailableSeats,
                    pd.current_bookings                AS BookedCount,
                    pd.max_group_size                  AS Capacity,
                    pd.status                          AS Status,
                    g.full_name                        AS GuideName,
                    CASE
                        WHEN v.model IS NOT NULL AND v.model <> ''
                        THEN v.vehicle_type || ' – ' || v.model
                        ELSE v.vehicle_type
                    END                                AS VehicleInfo
                FROM package_departures pd
                JOIN itineraries i ON i.id = pd.itinerary_id
                LEFT JOIN guides   g ON g.id = pd.guide_id
                LEFT JOIN vehicles v ON v.id = pd.vehicle_id
                WHERE pd.itinerary_id = @ItineraryId
                  AND pd.available_seats >= @RequiredSeats
                  AND pd.status NOT IN ('Cancelled', 'Completed')
                ORDER BY pd.departure_date;";

            return _dbConnection.Query<SuggestedDepartureItem>(sql, new { ItineraryId = itineraryId, RequiredSeats = requiredSeats }).ToList();
        }

        // ============================================================
        // CREATE DEPARTURE + ASSIGN BOOKINGS
        // ============================================================
        public DepartureDetailResponse CreateDeparture(CreateDepartureRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                int totalPersons = 0;
                if (request.BookingIds.Any())
                {
                    totalPersons = _dbConnection.QuerySingle<int>(
                        "SELECT COALESCE(SUM(total_person), 0) FROM itinerary_instances WHERE id = ANY(@Ids)",
                        new { Ids = request.BookingIds.ToArray() }, transaction);
                }

                if (totalPersons > request.Capacity)
                    throw new InvalidOperationException(
                        $"Total persons ({totalPersons}) exceeds departure capacity ({request.Capacity}).");

                const string insertSql = @"
                    INSERT INTO package_departures
                        (itinerary_id, departure_date, end_date, min_group_size, max_group_size,
                         current_bookings, available_seats, status, package_price,
                         is_guaranteed_departure, notes, guide_id, vehicle_id, created_at, updated_at)
                    VALUES
                        (@ItineraryId, @StartDate::date, @EndDate::date, 1, @Capacity,
                         @TotalPersons, @AvailableSeats, 'Confirmed', @PackagePrice,
                         true, @Notes, @GuideId, @VehicleId, NOW(), NOW())
                    RETURNING id;";

                long departureId = _dbConnection.QuerySingle<long>(insertSql, new
                {
                    request.ItineraryId,
                    request.StartDate,
                    request.EndDate,
                    request.Capacity,
                    TotalPersons = totalPersons,
                    AvailableSeats = request.Capacity - totalPersons,
                    request.PackagePrice,
                    request.Notes,
                    request.GuideId,
                    request.VehicleId
                }, transaction);

                if (request.BookingIds.Any())
                {
                    _dbConnection.Execute(
                        "UPDATE itinerary_instances SET departure_id = @DepartureId WHERE id = ANY(@Ids)",
                        new { DepartureId = departureId, Ids = request.BookingIds.ToArray() },
                        transaction);
                }

                transaction.Commit();
                return GetDepartureById(departureId)!;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // ASSIGN BOOKINGS TO EXISTING DEPARTURE
        // ============================================================
        public DepartureDetailResponse AssignBookingsToDeparture(AssignBookingsToDepartureRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                var departure = _dbConnection.QuerySingleOrDefault<dynamic>(
                    "SELECT id, max_group_size AS capacity, current_bookings AS booked_count FROM package_departures WHERE id = @Id",
                    new { Id = request.DepartureId }, transaction)
                    ?? throw new KeyNotFoundException($"Departure {request.DepartureId} not found.");

                int incomingPersons = _dbConnection.QuerySingle<int>(
                    "SELECT COALESCE(SUM(total_person), 0) FROM itinerary_instances WHERE id = ANY(@Ids) AND departure_id IS NULL",
                    new { Ids = request.BookingIds.ToArray() }, transaction);

                int newTotal = (int)departure.booked_count + incomingPersons;
                if (newTotal > (int)departure.capacity)
                    throw new InvalidOperationException(
                        $"Cannot assign: total persons ({newTotal}) would exceed capacity ({departure.capacity}).");

                _dbConnection.Execute(
                    "UPDATE itinerary_instances SET departure_id = @DepartureId WHERE id = ANY(@Ids) AND departure_id IS NULL",
                    new { DepartureId = request.DepartureId, Ids = request.BookingIds.ToArray() },
                    transaction);

                _dbConnection.Execute(
                    @"UPDATE package_departures
                      SET current_bookings = current_bookings + @Added,
                          available_seats  = available_seats  - @Added,
                          updated_at       = NOW()
                      WHERE id = @Id",
                    new { Added = incomingPersons, Id = request.DepartureId },
                    transaction);

                transaction.Commit();
                return GetDepartureById(request.DepartureId)!;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // ============================================================
        // GET DEPARTURE BY ID (detail with assigned bookings)
        // ============================================================
        public DepartureDetailResponse? GetDepartureById(long departureId)
        {
            const string sql = @"
                SELECT
                    pd.id                                  AS Id,
                    pd.itinerary_id                        AS ItineraryId,
                    i.title                                AS ItineraryTitle,
                    pd.departure_date                      AS DepartureDate,
                    pd.end_date                            AS EndDate,
                    pd.max_group_size                      AS Capacity,
                    pd.current_bookings                    AS BookedCount,
                    pd.available_seats                     AS AvailableSeats,
                    pd.status                              AS Status,
                    pd.package_price                       AS PackagePrice,
                    pd.guide_id                            AS GuideId,
                    g.full_name                            AS GuideName,
                    pd.vehicle_id                          AS VehicleId,
                    CASE
                        WHEN v.model IS NOT NULL AND v.model <> ''
                        THEN v.vehicle_type || ' – ' || v.model
                        ELSE v.vehicle_type
                    END                                    AS VehicleInfo,
                    pd.notes                               AS Notes,
                    pd.created_at                          AS CreatedAt
                FROM package_departures pd
                JOIN itineraries i ON i.id = pd.itinerary_id
                LEFT JOIN guides   g ON g.id = pd.guide_id
                LEFT JOIN vehicles v ON v.id = pd.vehicle_id
                WHERE pd.id = @Id;";

            var departure = _dbConnection.QuerySingleOrDefault<DepartureDetailResponse>(sql, new { Id = departureId });
            if (departure == null) return null;

            // fetch assigned bookings
            const string bookingsSql = @"
                SELECT
                    ii.id                          AS InstanceId,
                    ii.booking_reference           AS BookingReference,
                    ii.template_itinerary_id       AS ItineraryId,
                    i2.title                       AS ItineraryTitle,
                    ii.start_date                  AS StartDate,
                    ii.end_date                    AS EndDate,
                    ii.total_person                AS TotalPerson,
                    ii.status                      AS Status,
                    ii.payment_status              AS PaymentStatus,
                    ii.total_amount                AS TotalAmount,
                    ii.created_at                  AS CreatedAt,
                    t.full_name                    AS LeadTravelerName,
                    t.email                        AS LeadTravelerEmail,
                    t.contact_number               AS LeadTravelerPhone
                FROM itinerary_instances ii
                JOIN itineraries i2 ON i2.id = ii.template_itinerary_id
                LEFT JOIN LATERAL (
                    SELECT full_name, email, contact_number
                    FROM travelers
                    WHERE itinerary_instance_id = ii.id
                    ORDER BY id LIMIT 1
                ) t ON true
                WHERE ii.departure_id = @DepartureId
                ORDER BY ii.created_at;";

            departure.AssignedBookings = _dbConnection.Query<UnassignedBookingItem>(bookingsSql, new { DepartureId = departureId }).ToList();
            return departure;
        }

        // ============================================================
        // GET TRAVELER NOTIFICATION INFO
        // ============================================================
        public List<TravelerNotificationInfo> GetTravelerNotifications(long departureId)
        {
            const string sql = @"
                SELECT
                    ii.id                          AS BookingInstanceId,
                    ii.booking_reference           AS BookingReference,
                    t.full_name                    AS TravelerName,
                    t.email                        AS TravelerEmail,
                    t.contact_number               AS TravelerPhone,
                    i.title                        AS ItineraryTitle,
                    pd.departure_date              AS StartDate,
                    pd.end_date                    AS EndDate,
                    g.full_name                    AS GuideName,
                    CASE
                        WHEN v.model IS NOT NULL AND v.model <> ''
                        THEN v.vehicle_type || ' – ' || v.model
                        ELSE v.vehicle_type
                    END                 AS VehicleInfo,
                    pd.current_bookings AS TotalGroupSize
                FROM package_departures pd
                JOIN itinerary_instances ii ON ii.departure_id = pd.id
                JOIN itineraries i ON i.id = pd.itinerary_id
                JOIN travelers t ON t.itinerary_instance_id = ii.id
                LEFT JOIN guides   g ON g.id  = pd.guide_id
                LEFT JOIN vehicles v ON v.id  = pd.vehicle_id
                WHERE pd.id = @DepartureId
                ORDER BY ii.id, t.id;";

            return _dbConnection.Query<TravelerNotificationInfo>(sql, new { DepartureId = departureId }).ToList();
        }
        // ============================================================
        // GET ALL DEPARTURES (with guide, vehicle, traveler aggregates)
        // ============================================================
        public List<DepartureListItem> GetAllDepartures()
        {
            const string sql = @"
                SELECT
                    pd.id                                       AS Id,
                    pd.itinerary_id                             AS ItineraryId,
                    i.title                                     AS ItineraryTitle,
                    pd.departure_date                           AS DepartureDate,
                    pd.end_date                                 AS EndDate,
                    pd.max_group_size                           AS Capacity,
                    pd.current_bookings                         AS BookedCount,
                    pd.available_seats                          AS AvailableSeats,
                    pd.status                                   AS Status,
                    pd.package_price                            AS PackagePrice,
                    pd.guide_id                                 AS GuideId,
                    g.full_name                                 AS GuideName,
                    g.specialization                            AS GuideSpecialization,
                    pd.vehicle_id                               AS VehicleId,
                    CASE
                        WHEN v.model IS NOT NULL AND v.model <> ''
                        THEN v.vehicle_type || ' – ' || v.model
                        ELSE v.vehicle_type
                    END                                         AS VehicleInfo,
                    COUNT(DISTINCT ii.id)                       AS TravelerCount,
                    STRING_AGG(DISTINCT t.full_name, ', '
                        ORDER BY t.full_name)                   AS TravelerNames,
                    pd.notes                                    AS Notes,
                    pd.created_at                               AS CreatedAt
                FROM package_departures pd
                JOIN itineraries i ON i.id = pd.itinerary_id
                LEFT JOIN guides   g ON g.id = pd.guide_id
                LEFT JOIN vehicles v ON v.id = pd.vehicle_id
                LEFT JOIN itinerary_instances ii ON ii.departure_id = pd.id
                LEFT JOIN LATERAL (
                    SELECT full_name
                    FROM travelers
                    WHERE itinerary_instance_id = ii.id
                    ORDER BY id LIMIT 1
                ) t ON ii.id IS NOT NULL
                GROUP BY
                    pd.id, i.title, g.full_name, g.specialization,
                    v.vehicle_type, v.model
                ORDER BY pd.departure_date DESC;";

            return _dbConnection.Query<DepartureListItem>(sql).ToList();
        }
    }
}
