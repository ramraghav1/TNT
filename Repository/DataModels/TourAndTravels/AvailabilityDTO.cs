using System;
using System.Collections.Generic;

namespace Repository.DataModels.TourAndTravels
{
    // ===========================
    // AVAILABILITY
    // ===========================
    public class AvailabilityDTO
    {
        public long Id { get; set; }
        public string InventoryType { get; set; } = string.Empty;
        public long InventoryId { get; set; }
        public DateTime Date { get; set; }
        public int TotalCapacity { get; set; }
        public int BookedCapacity { get; set; }
        public int AvailableCapacity { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? SpecialPrice { get; set; }
        public string? Notes { get; set; }
    }

    // ===========================
    // AVAILABILITY BLOCKS
    // ===========================
    public class AvailabilityBlockDTO
    {
        public long Id { get; set; }
        public string InventoryType { get; set; } = string.Empty;
        public long InventoryId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===========================
    // PACKAGE DEPARTURES
    // ===========================
    public class PackageDepartureDTO
    {
        public long Id { get; set; }
        public long ItineraryId { get; set; }
        public DateOnly DepartureDate { get; set; }
        public int MinGroupSize { get; set; }
        public int MaxGroupSize { get; set; }
        public int CurrentBookings { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal PackagePrice { get; set; }
        public bool IsGuaranteedDeparture { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===========================
    // BOOKING INVENTORY LINK
    // ===========================
    public class BookingInventoryDTO
    {
        public long Id { get; set; }
        public long BookingInstanceId { get; set; }
        public string InventoryType { get; set; } = string.Empty;
        public long InventoryId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
