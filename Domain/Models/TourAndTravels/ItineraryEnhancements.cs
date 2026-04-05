using System;
using System.Collections.Generic;

namespace Domain.Models.TourAndTravels
{
    /// <summary>
    /// Enhanced itinerary models for media, maps, and booking management
    /// </summary>
    public class ItineraryEnhancements
    {
        // ============================================================
        // ITINERARY MEDIA (Marketing)
        // ============================================================
        
        public class ItineraryMedia
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string MediaType { get; set; } = string.Empty; // Image, Video, VideoLink
            public string MediaUrl { get; set; } = string.Empty;
            public string? Caption { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsFeatured { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class CreateMediaRequest
        {
            public string MediaType { get; set; } = string.Empty; // Image, Video, VideoLink
            public string MediaUrl { get; set; } = string.Empty;
            public string? Caption { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsFeatured { get; set; }
        }

        public class MediaResponse
        {
            public long Id { get; set; }
            public string MediaType { get; set; } = string.Empty;
            public string MediaUrl { get; set; } = string.Empty;
            public string? Caption { get; set; }
            public int DisplayOrder { get; set; }
            public bool IsFeatured { get; set; }
        }

        // ============================================================
        // INSTANCE DAY ASSIGNMENTS (Hotels, Guides, Vehicles)
        // ============================================================
        
        public class InstanceDayAssignment
        {
            public long Id { get; set; }
            public long InstanceDayId { get; set; }
            public long? HotelId { get; set; }
            public string? RoomType { get; set; }
            public int? NumberOfRooms { get; set; }
            public long? GuideId { get; set; }
            public long? VehicleId { get; set; }
            public string? Notes { get; set; }
            public DateTime AssignedAt { get; set; }
            public string? AssignedBy { get; set; }
        }

        public class CreateAssignmentRequest
        {
            public long InstanceDayId { get; set; }
            public long? HotelId { get; set; }
            public string? RoomType { get; set; }
            public int? NumberOfRooms { get; set; }
            public long? GuideId { get; set; }
            public long? VehicleId { get; set; }
            public string? Notes { get; set; }
        }

        public class AssignmentResponse
        {
            public long Id { get; set; }
            public long InstanceDayId { get; set; }
            public int DayNumber { get; set; }
            public DateTime? Date { get; set; }
            public HotelAssignment? Hotel { get; set; }
            public GuideAssignment? Guide { get; set; }
            public VehicleAssignment? Vehicle { get; set; }
            public string? Notes { get; set; }
            public string AssignedBy { get; set; } = string.Empty;
        }

        public class HotelAssignment
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? RoomType { get; set; }
            public int? NumberOfRooms { get; set; }
        }

        public class GuideAssignment
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Phone { get; set; }
        }

        public class VehicleAssignment
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? PlateNumber { get; set; }
        }

        // ============================================================
        // AVAILABILITY CHECKING
        // ============================================================
        
        public class CheckAvailabilityRequest
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<long>? HotelIds { get; set; }
            public List<long>? GuideIds { get; set; }
            public List<long>? VehicleIds { get; set; }
        }

        public class AvailabilityResponse
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<HotelAvailability> Hotels { get; set; } = new();
            public List<GuideAvailability> Guides { get; set; } = new();
            public List<VehicleAvailability> Vehicles { get; set; } = new();
        }

        public class HotelAvailability
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }
            public int? AvailableRooms { get; set; }
            public List<DateConflict> Conflicts { get; set; } = new();
        }

        public class GuideAvailability
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }
            public List<DateConflict> Conflicts { get; set; } = new();
        }

        public class VehicleAvailability
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsAvailable { get; set; }
            public List<DateConflict> Conflicts { get; set; } = new();
        }

        public class DateConflict
        {
            public DateTime Date { get; set; }
            public long BookingId { get; set; }
            public string BookingReference { get; set; } = string.Empty;
        }

        // ============================================================
        // BOOKING REQUESTS (from website)
        // ============================================================
        
        public class BookingRequest
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string? CustomerPhone { get; set; }
            public DateTime? PreferredStartDate { get; set; }
            public int NumberOfTravelers { get; set; }
            public string? Message { get; set; }
            public string Status { get; set; } = "Pending";
            public long? ConvertedToInstanceId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? RespondedAt { get; set; }
            public string? RespondedBy { get; set; }
        }

        public class CreateBookingRequestRequest
        {
            public long ItineraryId { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string? CustomerPhone { get; set; }
            public DateTime? PreferredStartDate { get; set; }
            public int NumberOfTravelers { get; set; }
            public string? Message { get; set; }
        }

        public class BookingRequestResponse
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerEmail { get; set; } = string.Empty;
            public string? CustomerPhone { get; set; }
            public DateTime? PreferredStartDate { get; set; }
            public int NumberOfTravelers { get; set; }
            public string? Message { get; set; }
            public string Status { get; set; } = string.Empty;
            public long? ConvertedToInstanceId { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ============================================================
        // ENHANCED ITINERARY (for public website)
        // ============================================================
        
        public class PublicItineraryResponse
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? ShortDescription { get; set; }
            public string? Description { get; set; }
            public int DurationDays { get; set; }
            public string? DifficultyLevel { get; set; }
            public string? Highlights { get; set; }
            public decimal? PriceStartingFrom { get; set; }
            public string? SeasonBestTime { get; set; }
            public int? MaxGroupSize { get; set; }
            public int? MinAgeRequirement { get; set; }
            public string? Tags { get; set; }
            public string? ThumbnailUrl { get; set; }
            public string? MapImageUrl { get; set; }
            public string? MapEmbedUrl { get; set; }
            public string? MapCoordinates { get; set; }
            public List<MediaResponse> Media { get; set; } = new();
            public List<DayPreview> Days { get; set; } = new();
        }

        public class DayPreview
        {
            public int DayNumber { get; set; }
            public string? Title { get; set; }
            public string? Location { get; set; }
            public List<string> Activities { get; set; } = new();
        }

        // ============================================================
        // BOOKING CREATION WITH ASSIGNMENTS
        // ============================================================
        
        public class CreateBookingWithAssignmentsRequest
        {
            public long TemplateItineraryId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string? SpecialRequests { get; set; }
            public List<DayAssignmentInput> DayAssignments { get; set; } = new();
            public TravelerInfo? Traveler { get; set; }
        }

        public class DayAssignmentInput
        {
            public int DayNumber { get; set; }
            public long? HotelId { get; set; }
            public string? RoomType { get; set; }
            public int? NumberOfRooms { get; set; }
            public long? GuideId { get; set; }
            public long? VehicleId { get; set; }
            public string? Notes { get; set; }
        }

        public class TravelerInfo
        {
            public string FullName { get; set; } = string.Empty;
            public string? ContactNumber { get; set; }
            public string? Email { get; set; }
            public string? Nationality { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Seniors { get; set; }
        }

        public class BookingWithAssignmentsResponse
        {
            public long InstanceId { get; set; }
            public string BookingReference { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public List<AssignmentConflict> Conflicts { get; set; } = new();
        }

        public class AssignmentConflict
        {
            public int DayNumber { get; set; }
            public DateTime Date { get; set; }
            public string ResourceType { get; set; } = string.Empty; // Hotel, Guide, Vehicle
            public long ResourceId { get; set; }
            public string ResourceName { get; set; } = string.Empty;
            public string ConflictReason { get; set; } = string.Empty;
        }
    }
}
