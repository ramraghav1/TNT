using System;
using System.Collections.Generic;

namespace Repository.DataModels.TourAndTravels
{
    public class DepartureManagementDTO
    {
        public class UnassignedBookingItem
        {
            public long InstanceId { get; set; }
            public string? BookingReference { get; set; }
            public long ItineraryId { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public DateOnly? StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public int TotalPerson { get; set; }
            public string Status { get; set; } = string.Empty;
            public string PaymentStatus { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? LeadTravelerName { get; set; }
            public string? LeadTravelerEmail { get; set; }
            public string? LeadTravelerPhone { get; set; }
        }

        public class CreateDepartureRequest
        {
            public long ItineraryId { get; set; }
            public string StartDate { get; set; } = string.Empty;
            public string EndDate { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public long? GuideId { get; set; }
            public long? VehicleId { get; set; }
            public List<long> BookingIds { get; set; } = new();
            public decimal PackagePrice { get; set; }
            public string? Notes { get; set; }
        }

        public class AssignBookingsToDepartureRequest
        {
            public long DepartureId { get; set; }
            public List<long> BookingIds { get; set; } = new();
        }

        public class DepartureDetailResponse
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public DateOnly DepartureDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public int Capacity { get; set; }
            public int BookedCount { get; set; }
            public int AvailableSeats { get; set; }
            public string Status { get; set; } = string.Empty;
            public decimal PackagePrice { get; set; }
            public long? GuideId { get; set; }
            public string? GuideName { get; set; }
            public long? VehicleId { get; set; }
            public string? VehicleInfo { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public List<UnassignedBookingItem> AssignedBookings { get; set; } = new();
        }

        public class SuggestedDepartureItem
        {
            public long Id { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public DateOnly DepartureDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public int AvailableSeats { get; set; }
            public int BookedCount { get; set; }
            public int Capacity { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? GuideName { get; set; }
            public string? VehicleInfo { get; set; }
        }

        public class DepartureListItem
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public DateOnly DepartureDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public int Capacity { get; set; }
            public int BookedCount { get; set; }
            public int AvailableSeats { get; set; }
            public string Status { get; set; } = string.Empty;
            public decimal PackagePrice { get; set; }
            public long? GuideId { get; set; }
            public string? GuideName { get; set; }
            public string? GuideSpecialization { get; set; }
            public long? VehicleId { get; set; }
            public string? VehicleInfo { get; set; }
            public int TravelerCount { get; set; }
            public string? TravelerNames { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class TravelerNotificationInfo
        {
            public long BookingInstanceId { get; set; }
            public string? BookingReference { get; set; }
            public string TravelerName { get; set; } = string.Empty;
            public string? TravelerEmail { get; set; }
            public string? TravelerPhone { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public DateOnly StartDate { get; set; }
            public DateOnly? EndDate { get; set; }
            public string? GuideName { get; set; }
            public string? VehicleInfo { get; set; }
            public int TotalGroupSize { get; set; }
        }
    }
}
