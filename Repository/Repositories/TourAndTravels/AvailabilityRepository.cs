using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;
using Repository.Interfaces.TourAndTravels;

namespace Repository.Repositories.TourAndTravels
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly IDbConnection _dbConnection;

        public AvailabilityRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // AVAILABILITY CHECKING
        // ============================================================
        public List<Availability.AvailabilityResponse> CheckAvailability(Availability.CheckAvailabilityRequest request)
        {
            string query = @"
                SELECT 
                    a.date, a.inventory_type, a.inventory_id, a.total_capacity, 
                    a.booked_capacity, a.available_capacity, a.status, a.special_price
                FROM availability a
                WHERE a.inventory_type = @InventoryType
                    AND (@InventoryId IS NULL OR a.inventory_id = @InventoryId)
                    AND a.date BETWEEN @StartDate AND @EndDate
                    AND a.status != 'Blocked'
                    AND a.available_capacity >= @RequiredCapacity
                ORDER BY a.date, a.inventory_id;";

            var availabilityRecords = _dbConnection.Query<AvailabilityDTO>(
                query,
                new
                {
                    request.InventoryType,
                    request.InventoryId,
                    request.StartDate,
                    request.EndDate,
                    request.RequiredCapacity
                }
            ).ToList();

            // Get inventory names
            var result = new List<Availability.AvailabilityResponse>();
            foreach (var record in availabilityRecords)
            {
                string inventoryName = GetInventoryName(record.InventoryType, record.InventoryId);
                
                result.Add(new Availability.AvailabilityResponse
                {
                    Date = record.Date,
                    InventoryType = record.InventoryType,
                    InventoryId = record.InventoryId,
                    InventoryName = inventoryName,
                    TotalCapacity = record.TotalCapacity,
                    BookedCapacity = record.BookedCapacity,
                    AvailableCapacity = record.AvailableCapacity,
                    Status = record.Status,
                    Price = record.SpecialPrice,
                    IsAvailable = record.AvailableCapacity >= request.RequiredCapacity
                });
            }

            return result;
        }

        // ============================================================
        // AVAILABILITY INITIALIZATION
        // ============================================================
        public bool InitializeAvailability(string inventoryType, long inventoryId, DateTime startDate, DateTime endDate, int capacity)
        {
            var dates = new List<DateTime>();
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                dates.Add(date);
            }

            string insertQuery = @"
                INSERT INTO availability
                (inventory_type, inventory_id, date, total_capacity, booked_capacity, available_capacity, status)
                VALUES
                (@InventoryType, @InventoryId, @Date, @Capacity, 0, @Capacity, 'Available')
                ON CONFLICT (inventory_type, inventory_id, date) DO NOTHING;";

            foreach (var date in dates)
            {
                _dbConnection.Execute(
                    insertQuery,
                    new
                    {
                        InventoryType = inventoryType,
                        InventoryId = inventoryId,
                        Date = date,
                        Capacity = capacity
                    }
                );
            }

            return true;
        }

        // ============================================================
        // CAPACITY MANAGEMENT
        // ============================================================
        public bool UpdateCapacity(string inventoryType, long inventoryId, DateTime date, int change)
        {
            string query = @"
                UPDATE availability
                SET booked_capacity = booked_capacity + @Change,
                    available_capacity = available_capacity - @Change,
                    status = CASE 
                        WHEN (available_capacity - @Change) <= 0 THEN 'Full'
                        ELSE 'Available'
                    END
                WHERE inventory_type = @InventoryType
                    AND inventory_id = @InventoryId
                    AND date = @Date;";

            int rowsAffected = _dbConnection.Execute(
                query,
                new
                {
                    InventoryType = inventoryType,
                    InventoryId = inventoryId,
                    Date = date.Date,
                    Change = change
                }
            );

            return rowsAffected > 0;
        }

        // ============================================================
        // AVAILABILITY BLOCKS
        // ============================================================
        public Availability.AvailabilityBlockResponse CreateBlock(Availability.BlockAvailabilityRequest request, long userId)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert block record
                    string insertBlockQuery = @"
                        INSERT INTO availability_blocks
                        (inventory_type, inventory_id, start_date, end_date, reason, notes, created_by)
                        VALUES
                        (@InventoryType, @InventoryId, @StartDate, @EndDate, @Reason, @Notes, @CreatedBy)
                        RETURNING id;";

                    long blockId = _dbConnection.QuerySingle<long>(
                        insertBlockQuery,
                        new
                        {
                            request.InventoryType,
                            request.InventoryId,
                            request.StartDate,
                            request.EndDate,
                            request.Reason,
                            request.Notes,
                            CreatedBy = userId
                        },
                        transaction
                    );

                    // Update availability status for affected dates
                    string updateAvailabilityQuery = @"
                        UPDATE availability
                        SET status = 'Blocked'
                        WHERE inventory_type = @InventoryType
                            AND inventory_id = @InventoryId
                            AND date BETWEEN @StartDate AND @EndDate;";

                    _dbConnection.Execute(
                        updateAvailabilityQuery,
                        new
                        {
                            request.InventoryType,
                            request.InventoryId,
                            request.StartDate,
                            request.EndDate
                        },
                        transaction
                    );

                    transaction.Commit();

                    // Return the created block
                    string selectQuery = @"
                        SELECT id, inventory_type, inventory_id, start_date, end_date, reason, notes, created_by, created_at
                        FROM availability_blocks
                        WHERE id = @BlockId;";

                    var block = _dbConnection.QuerySingle<AvailabilityBlockDTO>(selectQuery, new { BlockId = blockId });

                    string inventoryName = GetInventoryName(block.InventoryType, block.InventoryId);

                    return new Availability.AvailabilityBlockResponse
                    {
                        Id = block.Id,
                        InventoryType = block.InventoryType,
                        InventoryId = block.InventoryId,
                        InventoryName = inventoryName,
                        StartDate = block.StartDate.ToDateTime(TimeOnly.MinValue),
                        EndDate = block.EndDate.ToDateTime(TimeOnly.MinValue),
                        Reason = block.Reason,
                        Notes = block.Notes,
                        CreatedAt = block.CreatedAt
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<Availability.AvailabilityBlockResponse> GetBlocks(DateTime? startDate = null, DateTime? endDate = null)
        {
            string query = @"
                SELECT id, inventory_type, inventory_id, start_date, end_date, reason, notes, created_by, created_at
                FROM availability_blocks
                WHERE (@StartDate IS NULL OR end_date >= @StartDate)
                    AND (@EndDate IS NULL OR start_date <= @EndDate)
                ORDER BY start_date DESC;";

            var blocks = _dbConnection.Query<AvailabilityBlockDTO>(
                query,
                new { StartDate = startDate, EndDate = endDate }
            ).ToList();

            return blocks.Select(b => new Availability.AvailabilityBlockResponse
            {
                Id = b.Id,
                InventoryType = b.InventoryType,
                InventoryId = b.InventoryId,
                InventoryName = GetInventoryName(b.InventoryType, b.InventoryId),
                StartDate = b.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = b.EndDate.ToDateTime(TimeOnly.MinValue),
                Reason = b.Reason,
                Notes = b.Notes,
                CreatedAt = b.CreatedAt
            }).ToList();
        }

        public bool DeleteBlock(long blockId)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Get block details first
                    string selectQuery = @"
                        SELECT inventory_type, inventory_id, start_date, end_date
                        FROM availability_blocks
                        WHERE id = @BlockId;";

                    var block = _dbConnection.QuerySingleOrDefault<AvailabilityBlockDTO>(selectQuery, new { BlockId = blockId }, transaction);

                    if (block == null)
                        return false;

                    // Delete the block
                    string deleteQuery = "DELETE FROM availability_blocks WHERE id = @BlockId;";
                    _dbConnection.Execute(deleteQuery, new { BlockId = blockId }, transaction);

                    // Update availability status back to Available (if not fully booked)
                    string updateAvailabilityQuery = @"
                        UPDATE availability
                        SET status = CASE 
                            WHEN available_capacity > 0 THEN 'Available'
                            ELSE 'Full'
                        END
                        WHERE inventory_type = @InventoryType
                            AND inventory_id = @InventoryId
                            AND date BETWEEN @StartDate AND @EndDate
                            AND status = 'Blocked';";

                    _dbConnection.Execute(
                        updateAvailabilityQuery,
                        new
                        {
                            block.InventoryType,
                            block.InventoryId,
                            block.StartDate,
                            block.EndDate
                        },
                        transaction
                    );

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // ============================================================
        // CALENDAR VIEW
        // ============================================================
        public Availability.CalendarViewResponse GetCalendarView(Availability.CalendarViewRequest request)
        {
            var response = new Availability.CalendarViewResponse();
            
            // Get booking events with assigned guide / vehicle / hotel names
            string bookingQuery = @"
                SELECT 
                    ii.id,
                    ii.template_itinerary_id,
                    ii.start_date,
                    ii.end_date,
                    ii.status,
                    i.title,
                    STRING_AGG(DISTINCT g.full_name, ', ')  AS guide_names,
                    STRING_AGG(DISTINCT
                        CASE WHEN v.model IS NOT NULL AND v.model <> ''
                             THEN v.vehicle_type || ' - ' || v.model
                             ELSE v.vehicle_type
                        END, ', ')                          AS vehicle_names,
                    STRING_AGG(DISTINCT h.name, ', ')       AS hotel_names
                FROM itinerary_instances ii
                JOIN itineraries i ON i.id = ii.template_itinerary_id
                LEFT JOIN itinerary_instance_days iid ON iid.itinerary_instance_id = ii.id
                LEFT JOIN itinerary_instance_day_assignments a ON a.instance_day_id = iid.id
                LEFT JOIN guides  g ON g.id = a.guide_id
                LEFT JOIN vehicles v ON v.id = a.vehicle_id
                LEFT JOIN hotels   h ON h.id = a.hotel_id
                WHERE ii.start_date <= @EndDate AND ii.end_date >= @StartDate
                GROUP BY ii.id, ii.template_itinerary_id, ii.start_date, ii.end_date, ii.status, i.title
                ORDER BY ii.start_date;";

            var bookings = _dbConnection.Query(bookingQuery, new { request.StartDate, request.EndDate }).ToList();

            foreach (var booking in bookings)
            {
                response.Events.Add(new Availability.CalendarEvent
                {
                    Id = booking.id,
                    EventType = "Booking",
                    Title = booking.title,
                    StartDate = ((DateOnly)booking.start_date).ToDateTime(TimeOnly.MinValue),
                    EndDate = ((DateOnly)booking.end_date).ToDateTime(TimeOnly.MinValue),
                    Status = booking.status,
                    Color = GetStatusColor(booking.status),
                    LinkedBookingId = booking.id,
                    GuideName   = string.IsNullOrWhiteSpace((string?)booking.guide_names)   ? null : (string)booking.guide_names,
                    VehicleName = string.IsNullOrWhiteSpace((string?)booking.vehicle_names) ? null : (string)booking.vehicle_names,
                    HotelName   = string.IsNullOrWhiteSpace((string?)booking.hotel_names)   ? null : (string)booking.hotel_names
                });
            }

            // Get block events
            var blocks = GetBlocks(request.StartDate, request.EndDate);
            foreach (var block in blocks)
            {
                response.Events.Add(new Availability.CalendarEvent
                {
                    Id = block.Id,
                    EventType = "Block",
                    InventoryType = block.InventoryType,
                    InventoryId = block.InventoryId,
                    Title = $"{block.Reason} - {block.InventoryName}",
                    StartDate = block.StartDate,
                    EndDate = block.EndDate,
                    Status = "Blocked",
                    Color = "#FF6B6B",
                    Description = block.Notes
                });
            }

            return response;
        }

        // ============================================================
        // PACKAGE DEPARTURES
        // ============================================================
        public Availability.PackageDepartureResponse CreatePackageDeparture(Availability.CreatePackageDepartureRequest request)
        {
            string insertQuery = @"
                INSERT INTO package_departures
                (itinerary_id, departure_date, min_group_size, max_group_size, 
                 current_bookings, available_seats, status, package_price, is_guaranteed_departure, notes)
                VALUES
                (@ItineraryId, @DepartureDate, @MinGroupSize, @MaxGroupSize,
                 0, @MaxGroupSize, 'Open', @PackagePrice, @IsGuaranteedDeparture, @Notes)
                RETURNING id;";

            long departureId = _dbConnection.QuerySingle<long>(
                insertQuery,
                new
                {
                    request.ItineraryId,
                    request.DepartureDate,
                    request.MinGroupSize,
                    request.MaxGroupSize,
                    request.PackagePrice,
                    request.IsGuaranteedDeparture,
                    request.Notes
                }
            );

            var result = GetPackageDepartureById(departureId);
            if (result == null)
                throw new Exception("Failed to retrieve created package departure");

            return result;
        }

        public List<Availability.PackageDepartureResponse> GetPackageDepartures(long? itineraryId = null)
        {
            string query = @"
                SELECT 
                    pd.id, pd.itinerary_id, pd.departure_date, pd.min_group_size, pd.max_group_size,
                    pd.current_bookings, pd.available_seats, pd.status, pd.package_price,
                    pd.is_guaranteed_departure, pd.notes, pd.created_at,
                    i.title as itinerary_title
                FROM package_departures pd
                JOIN itineraries i ON i.id = pd.itinerary_id
                WHERE (@ItineraryId IS NULL OR pd.itinerary_id = @ItineraryId)
                ORDER BY pd.departure_date;";

            var departures = _dbConnection.Query<dynamic>(query, new { ItineraryId = itineraryId }).ToList();

            return departures.Select(d => new Availability.PackageDepartureResponse
            {
                Id = d.id,
                ItineraryId = d.itinerary_id,
                ItineraryTitle = d.itinerary_title,
                DepartureDate = ((DateOnly)d.departure_date).ToDateTime(TimeOnly.MinValue),
                MinGroupSize = d.min_group_size,
                MaxGroupSize = d.max_group_size,
                CurrentBookings = d.current_bookings,
                AvailableSeats = d.available_seats,
                Status = d.status,
                PackagePrice = d.package_price,
                IsGuaranteedDeparture = d.is_guaranteed_departure,
                CanStillBook = d.available_seats > 0 && d.status == "Open",
                Notes = d.notes,
                CreatedAt = d.created_at
            }).ToList();
        }

        public Availability.PackageDepartureResponse? GetPackageDepartureById(long id)
        {
            string query = @"
                SELECT 
                    pd.id, pd.itinerary_id, pd.departure_date, pd.min_group_size, pd.max_group_size,
                    pd.current_bookings, pd.available_seats, pd.status, pd.package_price,
                    pd.is_guaranteed_departure, pd.notes, pd.created_at,
                    i.title as itinerary_title
                FROM package_departures pd
                JOIN itineraries i ON i.id = pd.itinerary_id
                WHERE pd.id = @Id;";

            var departure = _dbConnection.QuerySingleOrDefault<dynamic>(query, new { Id = id });

            if (departure == null)
                return null;

            return new Availability.PackageDepartureResponse
            {
                Id = departure.id,
                ItineraryId = departure.itinerary_id,
                ItineraryTitle = departure.itinerary_title,
                DepartureDate = ((DateOnly)departure.departure_date).ToDateTime(TimeOnly.MinValue),
                MinGroupSize = departure.min_group_size,
                MaxGroupSize = departure.max_group_size,
                CurrentBookings = departure.current_bookings,
                AvailableSeats = departure.available_seats,
                Status = departure.status,
                PackagePrice = departure.package_price,
                IsGuaranteedDeparture = departure.is_guaranteed_departure,
                CanStillBook = departure.available_seats > 0 && departure.status == "Open",
                Notes = departure.notes,
                CreatedAt = departure.created_at
            };
        }

        public Availability.PackageDepartureResponse? UpdatePackageDeparture(long id, Availability.UpdatePackageDepartureRequest request)
        {
            var updateFields = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (request.DepartureDate.HasValue)
            {
                updateFields.Add("departure_date = @DepartureDate");
                parameters.Add("DepartureDate", request.DepartureDate);
            }
            if (request.MinGroupSize.HasValue)
            {
                updateFields.Add("min_group_size = @MinGroupSize");
                parameters.Add("MinGroupSize", request.MinGroupSize);
            }
            if (request.MaxGroupSize.HasValue)
            {
                updateFields.Add("max_group_size = @MaxGroupSize");
                parameters.Add("MaxGroupSize", request.MaxGroupSize);
                updateFields.Add("available_seats = @MaxGroupSize - current_bookings");
            }
            if (request.PackagePrice.HasValue)
            {
                updateFields.Add("package_price = @PackagePrice");
                parameters.Add("PackagePrice", request.PackagePrice);
            }
            if (request.IsGuaranteedDeparture.HasValue)
            {
                updateFields.Add("is_guaranteed_departure = @IsGuaranteedDeparture");
                parameters.Add("IsGuaranteedDeparture", request.IsGuaranteedDeparture);
            }
            if (request.Notes != null)
            {
                updateFields.Add("notes = @Notes");
                parameters.Add("Notes", request.Notes);
            }
            if (request.Status != null)
            {
                updateFields.Add("status = @Status");
                parameters.Add("Status", request.Status);
            }

            if (!updateFields.Any())
                return GetPackageDepartureById(id);

            string updateQuery = $@"
                UPDATE package_departures
                SET {string.Join(", ", updateFields)}
                WHERE id = @Id;";

            _dbConnection.Execute(updateQuery, parameters);

            return GetPackageDepartureById(id);
        }

        public bool DeletePackageDeparture(long id)
        {
            string query = "DELETE FROM package_departures WHERE id = @Id;";
            int rowsAffected = _dbConnection.Execute(query, new { Id = id });
            return rowsAffected > 0;
        }

        // ============================================================
        // BOOKING INVENTORY LINKS
        // ============================================================
        public Availability.BookingInventoryResponse AssignInventoryToBooking(Availability.AssignInventoryToBookingRequest request)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Insert booking inventory link
                    string insertQuery = @"
                        INSERT INTO booking_inventory
                        (booking_instance_id, inventory_type, inventory_id, start_date, end_date, quantity, price, notes)
                        VALUES
                        (@BookingInstanceId, @InventoryType, @InventoryId, @StartDate, @EndDate, @Quantity, @Price, @Notes)
                        RETURNING id;";

                    long linkId = _dbConnection.QuerySingle<long>(
                        insertQuery,
                        request,
                        transaction
                    );

                    // Update capacity for each date in the range
                    for (var date = request.StartDate.Date; date <= request.EndDate.Date; date = date.AddDays(1))
                    {
                        UpdateCapacity(request.InventoryType, request.InventoryId, date, request.Quantity);
                    }

                    transaction.Commit();

                    // Return the created link
                    string selectQuery = @"
                        SELECT id, booking_instance_id, inventory_type, inventory_id, 
                               start_date, end_date, quantity, price, notes, created_at
                        FROM booking_inventory
                        WHERE id = @LinkId;";

                    var link = _dbConnection.QuerySingle<BookingInventoryDTO>(selectQuery, new { LinkId = linkId });

                    string inventoryName = GetInventoryName(link.InventoryType, link.InventoryId);

                    return new Availability.BookingInventoryResponse
                    {
                        Id = link.Id,
                        BookingInstanceId = link.BookingInstanceId,
                        InventoryType = link.InventoryType,
                        InventoryId = link.InventoryId,
                        InventoryName = inventoryName,
                        StartDate = link.StartDate.ToDateTime(TimeOnly.MinValue),
                        EndDate = link.EndDate.ToDateTime(TimeOnly.MinValue),
                        Quantity = link.Quantity,
                        Price = link.Price,
                        Notes = link.Notes,
                        CreatedAt = link.CreatedAt
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public List<Availability.BookingInventoryResponse> GetBookingInventory(long bookingInstanceId)
        {
            string query = @"
                SELECT id, booking_instance_id, inventory_type, inventory_id, 
                       start_date, end_date, quantity, price, notes, created_at
                FROM booking_inventory
                WHERE booking_instance_id = @BookingInstanceId
                ORDER BY start_date;";

            var links = _dbConnection.Query<BookingInventoryDTO>(query, new { BookingInstanceId = bookingInstanceId }).ToList();

            return links.Select(l => new Availability.BookingInventoryResponse
            {
                Id = l.Id,
                BookingInstanceId = l.BookingInstanceId,
                InventoryType = l.InventoryType,
                InventoryId = l.InventoryId,
                InventoryName = GetInventoryName(l.InventoryType, l.InventoryId),
                StartDate = l.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = l.EndDate.ToDateTime(TimeOnly.MinValue),
                Quantity = l.Quantity,
                Price = l.Price,
                Notes = l.Notes,
                CreatedAt = l.CreatedAt
            }).ToList();
        }

        public bool RemoveInventoryFromBooking(long id)
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    // Get link details first
                    string selectQuery = @"
                        SELECT inventory_type, inventory_id, start_date, end_date, quantity
                        FROM booking_inventory
                        WHERE id = @Id;";

                    var link = _dbConnection.QuerySingleOrDefault<BookingInventoryDTO>(selectQuery, new { Id = id }, transaction);

                    if (link == null)
                        return false;

                    // Delete the link
                    string deleteQuery = "DELETE FROM booking_inventory WHERE id = @Id;";
                    _dbConnection.Execute(deleteQuery, new { Id = id }, transaction);

                    // Release capacity for each date in the range
                    var startDateTime = link.StartDate.ToDateTime(TimeOnly.MinValue);
                    var endDateTime = link.EndDate.ToDateTime(TimeOnly.MinValue);
                    for (var date = startDateTime; date <= endDateTime; date = date.AddDays(1))
                    {
                        UpdateCapacity(link.InventoryType, link.InventoryId, date, -link.Quantity);
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // ============================================================
        // HELPER METHODS
        // ============================================================
        private string GetInventoryName(string inventoryType, long inventoryId)
        {
            string query = inventoryType.ToLower() switch
            {
                "hotel" => "SELECT name FROM hotels WHERE id = @Id",
                "vehicle" => "SELECT model || ' (' || registration_number || ')' FROM vehicles WHERE id = @Id",
                "guide" => "SELECT full_name FROM guides WHERE id = @Id",
                "activity" => "SELECT name FROM activities WHERE id = @Id",
                _ => "SELECT 'Unknown'"
            };

            return _dbConnection.QuerySingleOrDefault<string>(query, new { Id = inventoryId }) ?? "Unknown";
        }

        private string GetStatusColor(string status)
        {
            return status switch
            {
                "Confirmed" => "#4CAF50",
                "Pending" => "#FFC107",
                "Cancelled" => "#F44336",
                "Completed" => "#2196F3",
                _ => "#9E9E9E"
            };
        }
    }
}
