using System;
using System.Collections.Generic;

namespace Domain.Models.TourAndTravels
{
    public class Availability
    {
        // ===========================
        // AVAILABILITY CHECKING
        // ===========================
        public class CheckAvailabilityRequest
        {
            public string InventoryType { get; set; } = string.Empty; // Hotel, Vehicle, Guide, Activity
            public long? InventoryId { get; set; } // Null = check all of that type
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int RequiredCapacity { get; set; } = 1;
        }

        public class AvailabilityResponse
        {
            public DateTime Date { get; set; }
            public string InventoryType { get; set; } = string.Empty;
            public long InventoryId { get; set; }
            public string InventoryName { get; set; } = string.Empty;
            public int TotalCapacity { get; set; }
            public int BookedCapacity { get; set; }
            public int AvailableCapacity { get; set; }
            public string Status { get; set; } = "Available";
            public decimal? Price { get; set; }
            public bool IsAvailable { get; set; }
        }

        // ===========================
        // AVAILABILITY BLOCKING
        // ===========================
        public class BlockAvailabilityRequest
        {
            public string InventoryType { get; set; } = string.Empty;
            public long InventoryId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Reason { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }

        public class AvailabilityBlockResponse
        {
            public long Id { get; set; }
            public string InventoryType { get; set; } = string.Empty;
            public long InventoryId { get; set; }
            public string InventoryName { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Reason { get; set; } = string.Empty;
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // CALENDAR VIEW
        // ===========================
        public class CalendarViewRequest
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string? InventoryType { get; set; }
            public long? InventoryId { get; set; }
        }

        public class CalendarEvent
        {
            public long Id { get; set; }
            public string EventType { get; set; } = string.Empty; // Booking, Block, Maintenance
            public string InventoryType { get; set; } = string.Empty;
            public long InventoryId { get; set; }
            public string Title { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public string? Color { get; set; }
            public string? Description { get; set; }
            public long? LinkedBookingId { get; set; }
        }

        public class CalendarViewResponse
        {
            public List<CalendarEvent> Events { get; set; } = new();
            public Dictionary<DateTime, DailyCapacitySummary> DailySummaries { get; set; } = new();
        }

        public class DailyCapacitySummary
        {
            public DateTime Date { get; set; }
            public int TotalHotels { get; set; }
            public int AvailableHotels { get; set; }
            public int TotalVehicles { get; set; }
            public int AvailableVehicles { get; set; }
            public int TotalGuides { get; set; }
            public int AvailableGuides { get; set; }
            public int TotalActivities { get; set; }
            public int AvailableActivities { get; set; }
        }

        // ===========================
        // PACKAGE DEPARTURES
        // ===========================
        public class CreatePackageDepartureRequest
        {
            public long ItineraryId { get; set; }
            public DateTime DepartureDate { get; set; }
            public int MinGroupSize { get; set; }
            public int MaxGroupSize { get; set; }
            public decimal PackagePrice { get; set; }
            public bool IsGuaranteedDeparture { get; set; }
            public string? Notes { get; set; }
        }

        public class UpdatePackageDepartureRequest
        {
            public DateTime? DepartureDate { get; set; }
            public int? MinGroupSize { get; set; }
            public int? MaxGroupSize { get; set; }
            public decimal? PackagePrice { get; set; }
            public bool? IsGuaranteedDeparture { get; set; }
            public string? Notes { get; set; }
            public string? Status { get; set; }
        }

        public class PackageDepartureResponse
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public DateTime DepartureDate { get; set; }
            public int MinGroupSize { get; set; }
            public int MaxGroupSize { get; set; }
            public int CurrentBookings { get; set; }
            public int AvailableSeats { get; set; }
            public string Status { get; set; } = string.Empty;
            public decimal PackagePrice { get; set; }
            public bool IsGuaranteedDeparture { get; set; }
            public bool CanStillBook { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // BOOKING INVENTORY LINKS
        // ===========================
        public class AssignInventoryToBookingRequest
        {
            public long BookingInstanceId { get; set; }
            public string InventoryType { get; set; } = string.Empty;
            public long InventoryId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Quantity { get; set; } = 1;
            public decimal Price { get; set; }
            public string? Notes { get; set; }
        }

        public class BookingInventoryResponse
        {
            public long Id { get; set; }
            public long BookingInstanceId { get; set; }
            public string InventoryType { get; set; } = string.Empty;
            public long InventoryId { get; set; }
            public string InventoryName { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
