using System;
using System.Collections.Generic;

namespace Domain.Models.TourAndTravels
{
    /// <summary>
    /// Models for the "Manage Bookings → Create Departure" workflow.
    /// </summary>
    public class DepartureManagement
    {
        // ===========================
        // Unassigned booking (single row returned from GetUnassignedBookings)
        // ===========================
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
            // primary traveler contact
            public string? LeadTravelerName { get; set; }
            public string? LeadTravelerEmail { get; set; }
            public string? LeadTravelerPhone { get; set; }
        }

        // ===========================
        // Grouped by itinerary (for the Manage Bookings page)
        // ===========================
        public class BookingGroupByItinerary
        {
            public long ItineraryId { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public int TotalBookings { get; set; }
            public int TotalPersons { get; set; }
            public List<UnassignedBookingItem> Bookings { get; set; } = new();
        }

        // ===========================
        // Create Departure request
        // ===========================
        public class CreateDepartureRequest
        {
            public long ItineraryId { get; set; }
            public string StartDate { get; set; } = string.Empty;
            public string EndDate { get; set; } = string.Empty;
            public int Capacity { get; set; }          // max_group_size
            public long? GuideId { get; set; }
            public long? VehicleId { get; set; }
            public List<long> BookingIds { get; set; } = new(); // itinerary_instance ids to assign
            public decimal PackagePrice { get; set; }
            public string? Notes { get; set; }
        }

        // ===========================
        // Assign existing departure to bookings
        // ===========================
        public class AssignBookingsToDepartureRequest
        {
            public long DepartureId { get; set; }
            public List<long> BookingIds { get; set; } = new();
        }

        // ===========================
        // Departure detail response
        // ===========================
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

        // ===========================
        // Suggested existing departures for auto-suggest
        // ===========================
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

        // ===========================
        // Package Departure list item (for the departures list page)
        // ===========================
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
            public string ComputedStatus { get; set; } = string.Empty; // Upcoming/Ongoing/Completed/Cancelled
            public decimal PackagePrice { get; set; }
            public long? GuideId { get; set; }
            public string? GuideName { get; set; }
            public string? GuideSpecialization { get; set; }
            public long? VehicleId { get; set; }
            public string? VehicleInfo { get; set; }
            public bool HasInventory { get; set; }  // guide or vehicle assigned
            public int TravelerCount { get; set; }
            public string? TravelerNames { get; set; } // comma-separated lead traveler names
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // Notification info (used internally after departure creation)
        // ===========================
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
