using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Newtonsoft.Json;
using static Domain.Models.TourAndTravels.ItineraryEnhancements;

namespace Repository.Repositories.TourAndTravels
{
    public interface IItineraryEnhancementsRepository
    {
        // Media Management
        long AddMedia(long itineraryId, CreateMediaRequest request);
        List<MediaResponse> GetMedia(long itineraryId);
        bool DeleteMedia(long mediaId);
        bool UpdateMediaOrder(long mediaId, int displayOrder);

        // Assignments
        long CreateAssignment(CreateAssignmentRequest request);
        AssignmentResponse? GetAssignment(long assignmentId);
        List<AssignmentResponse> GetAssignmentsForInstance(long instanceId);
        bool UpdateAssignment(long assignmentId, CreateAssignmentRequest request);
        bool DeleteAssignment(long assignmentId);

        // Availability Checking
        AvailabilityResponse CheckAvailability(CheckAvailabilityRequest request);
        List<long> GetConflictingHotels(DateTime startDate, DateTime endDate, List<long> hotelIds);
        List<long> GetConflictingGuides(DateTime startDate, DateTime endDate, List<long> guideIds);
        List<long> GetConflictingVehicles(DateTime startDate, DateTime endDate, List<long> vehicleIds);

        // Booking Requests
        long CreateBookingRequest(CreateBookingRequestRequest request);
        List<BookingRequestResponse> GetBookingRequests(string? status = null);
        BookingRequestResponse? GetBookingRequest(long id);
        bool UpdateBookingRequestStatus(long id, string status, string? respondedBy = null);
        bool ConvertBookingRequest(long requestId, long instanceId);

        // Public Itinerary
        PublicItineraryResponse? GetPublicItinerary(long itineraryId);
        List<PublicItineraryResponse> GetPublishedItineraries();

        // Helper Methods
        List<(long Id, int DayNumber, DateTime? Date)> GetInstanceDays(long instanceId);
        string GetHotelName(long hotelId);
        string GetGuideName(long guideId);
        string GetVehicleName(long vehicleId);
    }

    public class ItineraryEnhancementsRepository : IItineraryEnhancementsRepository
    {
        private readonly IDbConnection _dbConnection;

        public ItineraryEnhancementsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // ============================================================
        // MEDIA MANAGEMENT
        // ============================================================

        public long AddMedia(long itineraryId, CreateMediaRequest request)
        {
            var sql = @"
                INSERT INTO itinerary_media 
                    (itinerary_id, media_type, media_url, caption, display_order, is_featured, created_at)
                VALUES 
                    (@ItineraryId, @MediaType, @MediaUrl, @Caption, @DisplayOrder, @IsFeatured, NOW())
                RETURNING id";

            return _dbConnection.QuerySingle<long>(sql, new
            {
                ItineraryId = itineraryId,
                request.MediaType,
                request.MediaUrl,
                request.Caption,
                request.DisplayOrder,
                request.IsFeatured
            });
        }

        public List<MediaResponse> GetMedia(long itineraryId)
        {
            var sql = @"
                SELECT id, media_type as MediaType, media_url as MediaUrl, 
                       caption, display_order as DisplayOrder, is_featured as IsFeatured
                FROM itinerary_media
                WHERE itinerary_id = @ItineraryId
                ORDER BY display_order, created_at";

            return _dbConnection.Query<MediaResponse>(sql, new { ItineraryId = itineraryId }).ToList();
        }

        public bool DeleteMedia(long mediaId)
        {
            var sql = "DELETE FROM itinerary_media WHERE id = @Id";
            return _dbConnection.Execute(sql, new { Id = mediaId }) > 0;
        }

        public bool UpdateMediaOrder(long mediaId, int displayOrder)
        {
            var sql = "UPDATE itinerary_media SET display_order = @DisplayOrder WHERE id = @Id";
            return _dbConnection.Execute(sql, new { Id = mediaId, DisplayOrder = displayOrder }) > 0;
        }

        // ============================================================
        // ASSIGNMENTS
        // ============================================================

        public long CreateAssignment(CreateAssignmentRequest request)
        {
            var sql = @"
                INSERT INTO itinerary_instance_day_assignments 
                    (instance_day_id, hotel_id, room_type, number_of_rooms, 
                     guide_id, vehicle_id, notes, assigned_at)
                VALUES 
                    (@InstanceDayId, @HotelId, @RoomType, @NumberOfRooms, 
                     @GuideId, @VehicleId, @Notes, NOW())
                RETURNING id";

            return _dbConnection.QuerySingle<long>(sql, request);
        }

        public AssignmentResponse? GetAssignment(long assignmentId)
        {
            var sql = @"
                SELECT 
                    a.id, a.instance_day_id as InstanceDayId, a.notes,
                    d.day_number as DayNumber, d.date,
                    h.id as HotelId, h.name as HotelName, a.room_type as RoomType, 
                    a.number_of_rooms as NumberOfRooms,
                    g.id as GuideId, g.name as GuideName, g.contact_number as GuidePhone,
                    v.id as VehicleId, v.name as VehicleName, v.plate_number as VehiclePlateNumber,
                    a.assigned_by as AssignedBy
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                LEFT JOIN hotels h ON h.id = a.hotel_id
                LEFT JOIN guides g ON g.id = a.guide_id
                LEFT JOIN vehicles v ON v.id = a.vehicle_id
                WHERE a.id = @Id";

            var result = _dbConnection.Query<dynamic>(sql, new { Id = assignmentId }).FirstOrDefault();
            if (result == null) return null;

            return MapToAssignmentResponse(result);
        }

        public List<AssignmentResponse> GetAssignmentsForInstance(long instanceId)
        {
            var sql = @"
                SELECT 
                    a.id, a.instance_day_id as InstanceDayId, a.notes,
                    d.day_number as DayNumber, d.date,
                    h.id as HotelId, h.name as HotelName, a.room_type as RoomType, 
                    a.number_of_rooms as NumberOfRooms,
                    g.id as GuideId, g.name as GuideName, g.contact_number as GuidePhone,
                    v.id as VehicleId, v.name as VehicleName, v.plate_number as VehiclePlateNumber,
                    a.assigned_by as AssignedBy
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                LEFT JOIN hotels h ON h.id = a.hotel_id
                LEFT JOIN guides g ON g.id = a.guide_id
                LEFT JOIN vehicles v ON v.id = a.vehicle_id
                WHERE d.itinerary_instance_id = @InstanceId
                ORDER BY d.day_number";

            return _dbConnection.Query<dynamic>(sql, new { InstanceId = instanceId })
                .Select(MapToAssignmentResponse)
                .ToList();
        }

        public bool UpdateAssignment(long assignmentId, CreateAssignmentRequest request)
        {
            var sql = @"
                UPDATE itinerary_instance_day_assignments 
                SET hotel_id = @HotelId, room_type = @RoomType, number_of_rooms = @NumberOfRooms,
                    guide_id = @GuideId, vehicle_id = @VehicleId, notes = @Notes
                WHERE id = @Id";

            return _dbConnection.Execute(sql, new
            {
                Id = assignmentId,
                request.HotelId,
                request.RoomType,
                request.NumberOfRooms,
                request.GuideId,
                request.VehicleId,
                request.Notes
            }) > 0;
        }

        public bool DeleteAssignment(long assignmentId)
        {
            var sql = "DELETE FROM itinerary_instance_day_assignments WHERE id = @Id";
            return _dbConnection.Execute(sql, new { Id = assignmentId }) > 0;
        }

        // ============================================================
        // AVAILABILITY CHECKING
        // ============================================================

        public AvailabilityResponse CheckAvailability(CheckAvailabilityRequest request)
        {
            var response = new AvailabilityResponse
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            // Check Hotels
            if (request.HotelIds != null && request.HotelIds.Any())
            {
                var sql = @"
                    SELECT h.id, h.name, h.total_rooms
                    FROM hotels h
                    WHERE h.id = ANY(@HotelIds)";

                var hotels = _dbConnection.Query<dynamic>(sql, new { HotelIds = request.HotelIds.ToArray() });

                foreach (var hotel in hotels)
                {
                    var conflicts = GetHotelConflicts((long)hotel.id, request.StartDate, request.EndDate);
                    var totalBooked = conflicts.Sum(c => c.RoomsBooked);
                    var availableRooms = (int)hotel.total_rooms - totalBooked;

                    response.Hotels.Add(new HotelAvailability
                    {
                        Id = hotel.id,
                        Name = hotel.name,
                        IsAvailable = availableRooms > 0,
                        AvailableRooms = availableRooms,
                        Conflicts = conflicts.Select(c => new DateConflict
                        {
                            Date = c.Date,
                            BookingId = c.BookingId,
                            BookingReference = c.BookingReference
                        }).ToList()
                    });
                }
            }

            // Check Guides
            if (request.GuideIds != null && request.GuideIds.Any())
            {
                var guideSql = @"SELECT id, name FROM guides WHERE id = ANY(@GuideIds)";
                var guides = _dbConnection.Query<(long id, string name)>(guideSql, 
                    new { GuideIds = request.GuideIds.ToArray() });

                foreach (var guide in guides)
                {
                    var conflicts = GetGuideConflicts(guide.id, request.StartDate, request.EndDate);
                    response.Guides.Add(new GuideAvailability
                    {
                        Id = guide.id,
                        Name = guide.name,
                        IsAvailable = !conflicts.Any(),
                        Conflicts = conflicts.Select(c => new DateConflict
                        {
                            Date = c.Date,
                            BookingId = c.BookingId,
                            BookingReference = c.BookingReference
                        }).ToList()
                    });
                }
            }

            // Check Vehicles
            if (request.VehicleIds != null && request.VehicleIds.Any())
            {
                var vehicleSql = @"SELECT id, name FROM vehicles WHERE id = ANY(@VehicleIds)";
                var vehicles = _dbConnection.Query<(long id, string name)>(vehicleSql, 
                    new { VehicleIds = request.VehicleIds.ToArray() });

                foreach (var vehicle in vehicles)
                {
                    var conflicts = GetVehicleConflicts(vehicle.id, request.StartDate, request.EndDate);
                    response.Vehicles.Add(new VehicleAvailability
                    {
                        Id = vehicle.id,
                        Name = vehicle.name,
                        IsAvailable = !conflicts.Any(),
                        Conflicts = conflicts.Select(c => new DateConflict
                        {
                            Date = c.Date,
                            BookingId = c.BookingId,
                            BookingReference = c.BookingReference
                        }).ToList()
                    });
                }
            }

            return response;
        }

        public List<long> GetConflictingHotels(DateTime startDate, DateTime endDate, List<long> hotelIds)
        {
            var sql = @"
                SELECT DISTINCT a.hotel_id
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                JOIN itinerary_instances i ON i.id = d.itinerary_instance_id
                WHERE a.hotel_id = ANY(@HotelIds)
                  AND d.date IS NOT NULL
                  AND d.date BETWEEN @StartDate AND @EndDate
                  AND i.status NOT IN ('Cancelled', 'Rejected')";

            return _dbConnection.Query<long>(sql, new
            {
                HotelIds = hotelIds.ToArray(),
                StartDate = startDate,
                EndDate = endDate
            }).ToList();
        }

        public List<long> GetConflictingGuides(DateTime startDate, DateTime endDate, List<long> guideIds)
        {
            var sql = @"
                SELECT DISTINCT a.guide_id
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                JOIN itinerary_instances i ON i.id = d.itinerary_instance_id
                WHERE a.guide_id = ANY(@GuideIds)
                  AND d.date IS NOT NULL
                  AND d.date BETWEEN @StartDate AND @EndDate
                  AND i.status NOT IN ('Cancelled', 'Rejected')";

            return _dbConnection.Query<long>(sql, new
            {
                GuideIds = guideIds.ToArray(),
                StartDate = startDate,
                EndDate = endDate
            }).ToList();
        }

        public List<long> GetConflictingVehicles(DateTime startDate, DateTime endDate, List<long> vehicleIds)
        {
            var sql = @"
                SELECT DISTINCT a.vehicle_id
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                JOIN itinerary_instances i ON i.id = d.itinerary_instance_id
                WHERE a.vehicle_id = ANY(@VehicleIds)
                  AND d.date IS NOT NULL
                  AND d.date BETWEEN @StartDate AND @EndDate
                  AND i.status NOT IN ('Cancelled', 'Rejected')";

            return _dbConnection.Query<long>(sql, new
            {
                VehicleIds = vehicleIds.ToArray(),
                StartDate = startDate,
                EndDate = endDate
            }).ToList();
        }

        // ============================================================
        // BOOKING REQUESTS
        // ============================================================

        public long CreateBookingRequest(CreateBookingRequestRequest request)
        {
            var sql = @"
                INSERT INTO booking_requests 
                    (itinerary_id, customer_name, customer_email, customer_phone, 
                     preferred_start_date, number_of_travelers, message, status, created_at)
                VALUES 
                    (@ItineraryId, @CustomerName, @CustomerEmail, @CustomerPhone, 
                     @PreferredStartDate, @NumberOfTravelers, @Message, 'Pending', NOW())
                RETURNING id";

            return _dbConnection.QuerySingle<long>(sql, request);
        }

        public List<BookingRequestResponse> GetBookingRequests(string? status = null)
        {
            var sql = @"
                SELECT 
                    r.id, r.itinerary_id as ItineraryId, i.title as ItineraryTitle,
                    r.customer_name as CustomerName, r.customer_email as CustomerEmail, 
                    r.customer_phone as CustomerPhone, r.preferred_start_date as PreferredStartDate,
                    r.number_of_travelers as NumberOfTravelers, r.message, r.status,
                    r.converted_to_instance_id as ConvertedToInstanceId, r.created_at as CreatedAt
                FROM booking_requests r
                JOIN itineraries i ON i.id = r.itinerary_id
                WHERE (@Status IS NULL OR r.status = @Status)
                ORDER BY r.created_at DESC";

            return _dbConnection.Query<BookingRequestResponse>(sql, new { Status = status }).ToList();
        }

        public BookingRequestResponse? GetBookingRequest(long id)
        {
            var sql = @"
                SELECT 
                    r.id, r.itinerary_id as ItineraryId, i.title as ItineraryTitle,
                    r.customer_name as CustomerName, r.customer_email as CustomerEmail, 
                    r.customer_phone as CustomerPhone, r.preferred_start_date as PreferredStartDate,
                    r.number_of_travelers as NumberOfTravelers, r.message, r.status,
                    r.converted_to_instance_id as ConvertedToInstanceId, r.created_at as CreatedAt
                FROM booking_requests r
                JOIN itineraries i ON i.id = r.itinerary_id
                WHERE r.id = @Id";

            return _dbConnection.QuerySingleOrDefault<BookingRequestResponse>(sql, new { Id = id });
        }

        public bool UpdateBookingRequestStatus(long id, string status, string? respondedBy = null)
        {
            var sql = @"
                UPDATE booking_requests 
                SET status = @Status, responded_at = NOW(), responded_by = @RespondedBy
                WHERE id = @Id";

            return _dbConnection.Execute(sql, new { Id = id, Status = status, RespondedBy = respondedBy }) > 0;
        }

        public bool ConvertBookingRequest(long requestId, long instanceId)
        {
            var sql = @"
                UPDATE booking_requests 
                SET status = 'Converted', converted_to_instance_id = @InstanceId,
                    responded_at = NOW()
                WHERE id = @RequestId";

            return _dbConnection.Execute(sql, new { RequestId = requestId, InstanceId = instanceId }) > 0;
        }

        // ============================================================
        // PUBLIC ITINERARY
        // ============================================================

        public PublicItineraryResponse? GetPublicItinerary(long itineraryId)
        {
            var sql = @"
                SELECT 
                    id, title, short_description as ShortDescription, description,
                    duration_days as DurationDays, difficulty_level as DifficultyLevel,
                    highlights, price_starting_from as PriceStartingFrom,
                    season_best_time as SeasonBestTime, max_group_size as MaxGroupSize,
                    min_age_requirement as MinAgeRequirement, tags,
                    thumbnail_url as ThumbnailUrl, map_image_url as MapImageUrl,
                    map_embed_url as MapEmbedUrl, map_coordinates as MapCoordinates
                FROM itineraries
                WHERE id = @Id AND is_published = true AND is_active = true";

            var itinerary = _dbConnection.QuerySingleOrDefault<PublicItineraryResponse>(sql, 
                new { Id = itineraryId });

            if (itinerary == null) return null;

            // Load media
            itinerary.Media = GetMedia(itineraryId);

            // Load day previews
            var daySql = @"
                SELECT d.day_number as DayNumber, d.title, d.location
                FROM itinerary_days d
                WHERE d.itinerary_id = @ItineraryId
                ORDER BY d.day_number";

            var days = _dbConnection.Query<DayPreview>(daySql, new { ItineraryId = itineraryId }).ToList();

            foreach (var day in days)
            {
                var activitySql = @"
                    SELECT activity 
                    FROM itinerary_day_activities 
                    WHERE itinerary_day_id = (
                        SELECT id FROM itinerary_days 
                        WHERE itinerary_id = @ItineraryId AND day_number = @DayNumber
                    )";

                day.Activities = _dbConnection.Query<string>(activitySql, new
                {
                    ItineraryId = itineraryId,
                    DayNumber = day.DayNumber
                }).ToList();
            }

            itinerary.Days = days;

            return itinerary;
        }

        public List<PublicItineraryResponse> GetPublishedItineraries()
        {
            var sql = @"
                SELECT 
                    id, title, short_description as ShortDescription, description,
                    duration_days as DurationDays, difficulty_level as DifficultyLevel,
                    highlights, price_starting_from as PriceStartingFrom,
                    season_best_time as SeasonBestTime, max_group_size as MaxGroupSize,
                    min_age_requirement as MinAgeRequirement, tags,
                    thumbnail_url as ThumbnailUrl, map_image_url as MapImageUrl,
                    map_embed_url as MapEmbedUrl, map_coordinates as MapCoordinates
                FROM itineraries
                WHERE is_published = true AND is_active = true
                ORDER BY created_at DESC";

            var itineraries = _dbConnection.Query<PublicItineraryResponse>(sql).ToList();

            // Load featured media for each
            foreach (var itinerary in itineraries)
            {
                var mediaSql = @"
                    SELECT id, media_type as MediaType, media_url as MediaUrl, 
                           caption, display_order as DisplayOrder, is_featured as IsFeatured
                    FROM itinerary_media
                    WHERE itinerary_id = @ItineraryId AND is_featured = true
                    ORDER BY display_order
                    LIMIT 3";

                itinerary.Media = _dbConnection.Query<MediaResponse>(mediaSql, 
                    new { ItineraryId = itinerary.Id }).ToList();
            }

            return itineraries;
        }

        // ============================================================
        // HELPER METHODS
        // ============================================================

        public List<(long Id, int DayNumber, DateTime? Date)> GetInstanceDays(long instanceId)
        {
            var sql = @"
                SELECT id, day_number as DayNumber, date
                FROM itinerary_instance_days
                WHERE itinerary_instance_id = @InstanceId
                ORDER BY day_number";

            return _dbConnection.Query<(long, int, DateTime?)>(sql, new { InstanceId = instanceId }).ToList();
        }

        public string GetHotelName(long hotelId)
        {
            var sql = "SELECT name FROM hotels WHERE id = @HotelId";
            return _dbConnection.QueryFirstOrDefault<string>(sql, new { HotelId = hotelId }) ?? $"Hotel-{hotelId}";
        }

        public string GetGuideName(long guideId)
        {
            var sql = "SELECT name FROM guides WHERE id = @GuideId";
            return _dbConnection.QueryFirstOrDefault<string>(sql, new { GuideId = guideId }) ?? $"Guide-{guideId}";
        }

        public string GetVehicleName(long vehicleId)
        {
            var sql = "SELECT name FROM vehicles WHERE id = @VehicleId";
            return _dbConnection.QueryFirstOrDefault<string>(sql, new { VehicleId = vehicleId }) ?? $"Vehicle-{vehicleId}";
        }

        // ============================================================
        // PRIVATE HELPER METHODS
        // ============================================================

        private AssignmentResponse MapToAssignmentResponse(dynamic row)
        {
            var response = new AssignmentResponse
            {
                Id = row.id,
                InstanceDayId = row.instancedayid,
                DayNumber = row.daynumber,
                Date = row.date,
                Notes = row.notes,
                AssignedBy = row.assignedby ?? "System"
            };

            if (row.hotelid != null)
            {
                response.Hotel = new HotelAssignment
                {
                    Id = row.hotelid,
                    Name = row.hotelname,
                    RoomType = row.roomtype,
                    NumberOfRooms = row.numberofrooms
                };
            }

            if (row.guideid != null)
            {
                response.Guide = new GuideAssignment
                {
                    Id = row.guideid,
                    Name = row.guidename,
                    Phone = row.guidephone
                };
            }

            if (row.vehicleid != null)
            {
                response.Vehicle = new VehicleAssignment
                {
                    Id = row.vehicleid,
                    Name = row.vehiclename,
                    PlateNumber = row.vehicleplatenumber
                };
            }

            return response;
        }

        private List<(DateTime Date, long BookingId, string BookingReference, int RoomsBooked)> GetHotelConflicts(
            long hotelId, DateTime startDate, DateTime endDate)
        {
            var sql = @"
                SELECT d.date, i.id as BookingId, i.booking_reference as BookingReference,
                       COALESCE(a.number_of_rooms, 0) as RoomsBooked
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                JOIN itinerary_instances i ON i.id = d.itinerary_instance_id
                WHERE a.hotel_id = @HotelId
                  AND d.date IS NOT NULL
                  AND d.date BETWEEN @StartDate AND @EndDate
                  AND i.status NOT IN ('Cancelled', 'Rejected')";

            return _dbConnection.Query<(DateTime, long, string, int)>(sql, new
            {
                HotelId = hotelId,
                StartDate = startDate,
                EndDate = endDate
            }).ToList();
        }

        private List<(DateTime Date, long BookingId, string BookingReference)> GetGuideConflicts(
            long guideId, DateTime startDate, DateTime endDate)
        {
            var sql = @"
                SELECT d.date, i.id as BookingId, i.booking_reference as BookingReference
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                JOIN itinerary_instances i ON i.id = d.itinerary_instance_id
                WHERE a.guide_id = @GuideId
                  AND d.date IS NOT NULL
                  AND d.date BETWEEN @StartDate AND @EndDate
                  AND i.status NOT IN ('Cancelled', 'Rejected')";

            return _dbConnection.Query<(DateTime, long, string)>(sql, new
            {
                GuideId = guideId,
                StartDate = startDate,
                EndDate = endDate
            }).ToList();
        }

        private List<(DateTime Date, long BookingId, string BookingReference)> GetVehicleConflicts(
            long vehicleId, DateTime startDate, DateTime endDate)
        {
            var sql = @"
                SELECT d.date, i.id as BookingId, i.booking_reference as BookingReference
                FROM itinerary_instance_day_assignments a
                JOIN itinerary_instance_days d ON d.id = a.instance_day_id
                JOIN itinerary_instances i ON i.id = d.itinerary_instance_id
                WHERE a.vehicle_id = @VehicleId
                  AND d.date IS NOT NULL
                  AND d.date BETWEEN @StartDate AND @EndDate
                  AND i.status NOT IN ('Cancelled', 'Rejected')";

            return _dbConnection.Query<(DateTime, long, string)>(sql, new
            {
                VehicleId = vehicleId,
                StartDate = startDate,
                EndDate = endDate
            }).ToList();
        }
    }
}
