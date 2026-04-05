using System;
using System.Collections.Generic;

namespace Repository.DataModels.TourAndTravels
{
    // ===========================
    // HOTELS
    // ===========================
    public class HotelDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? StarRating { get; set; }
        public string? Category { get; set; }
        public string? Amenities { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<HotelRoomDTO> Rooms { get; set; } = new();
    }

    public class HotelRoomDTO
    {
        public long Id { get; set; }
        public long HotelId { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int TotalRooms { get; set; }
        public decimal PricePerNight { get; set; }
        public string? Features { get; set; }
    }

    // ===========================
    // VEHICLES
    // ===========================
    public class VehicleDTO
    {
        public long Id { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Features { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal? PricePerKm { get; set; }
        public string? DriverName { get; set; }
        public string? DriverContact { get; set; }
        public string? InsuranceNumber { get; set; }
        public DateOnly? InsuranceExpiry { get; set; }
        public string? PermitNumber { get; set; }
        public DateOnly? PermitExpiry { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===========================
    // GUIDES
    // ===========================
    public class GuideDTO
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? ExperienceYears { get; set; }
        public string? Languages { get; set; }
        public string? Specialization { get; set; }
        public string? CertificationNumber { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal? Rating { get; set; }
        public string? Bio { get; set; }
        public string? Photo { get; set; }
        public bool IsActive { get; set; }
        public int TripsCompleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===========================
    // ACTIVITIES
    // ===========================
    public class ActivityDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int DurationHours { get; set; }
        public string? DifficultyLevel { get; set; }
        public int? MaxParticipants { get; set; }
        public int? MinParticipants { get; set; }
        public string? Equipment { get; set; }
        public decimal PricePerPerson { get; set; }
        public string? Description { get; set; }
        public string? SafetyInstructions { get; set; }
        public string? Images { get; set; }
        public bool IsActive { get; set; }
        public int TotalBookings { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
