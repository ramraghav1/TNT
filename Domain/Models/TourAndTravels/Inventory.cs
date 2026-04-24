using System;
using System.Collections.Generic;

namespace Domain.Models.TourAndTravels
{
    public class Inventory
    {
        // ===========================
        // HOTELS
        // ===========================
        public class CreateHotelRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string? ContactPerson { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public int? StarRating { get; set; }
            public string Category { get; set; } = "Standard";
            public string? Description { get; set; }
            public List<string>? Amenities { get; set; }
            public List<HotelRoomRequest> Rooms { get; set; } = new();
        }

        public class UpdateHotelRequest
        {
            public string? Name { get; set; }
            public string? Location { get; set; }
            public string? Address { get; set; }
            public string? ContactPerson { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public int? StarRating { get; set; }
            public string? Category { get; set; }
            public string? Description { get; set; }
            public List<string>? Amenities { get; set; }
            public List<HotelRoomRequest>? Rooms { get; set; }
            public bool? IsActive { get; set; }
        }

        public class HotelRoomRequest
        {
            public string RoomType { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public int TotalRooms { get; set; }
            public decimal PricePerNight { get; set; }
            public decimal? PricePerNightUsd { get; set; }
            public decimal? PricePerNightInr { get; set; }
            public List<string>? Features { get; set; }
        }

        public class HotelResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string? ContactPerson { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public int? StarRating { get; set; }
            public string Category { get; set; } = string.Empty;
            public string? Description { get; set; }
            public List<string> Amenities { get; set; } = new();
            public bool IsActive { get; set; }
            public List<HotelRoomResponse> Rooms { get; set; } = new();
            public DateTime CreatedAt { get; set; }
        }

        public class HotelRoomResponse
        {
            public long Id { get; set; }
            public string RoomType { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public int TotalRooms { get; set; }
            public decimal PricePerNight { get; set; }
            public decimal? PricePerNightUsd { get; set; }
            public decimal? PricePerNightInr { get; set; }
            public List<string> Features { get; set; } = new();
            public bool IsActive { get; set; }
        }

        // ===========================
        // VEHICLES
        // ===========================
        public class CreateVehicleRequest
        {
            public string VehicleType { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string RegistrationNumber { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public List<string>? Features { get; set; }
            public decimal PricePerDay { get; set; }
            public decimal? PricePerDayUsd { get; set; }
            public decimal? PricePerDayInr { get; set; }
            public decimal? PricePerKm { get; set; }
            public decimal? PricePerKmUsd { get; set; }
            public decimal? PricePerKmInr { get; set; }
            public string? DriverName { get; set; }
            public string? DriverContact { get; set; }
            public string? InsuranceNumber { get; set; }
            public DateOnly? InsuranceExpiry { get; set; }
            public string? PermitNumber { get; set; }
            public DateOnly? PermitExpiry { get; set; }
            public string? Description { get; set; }
        }

        public class UpdateVehicleRequest
        {
            public string? VehicleType { get; set; }
            public string? Model { get; set; }
            public string? RegistrationNumber { get; set; }
            public int? Capacity { get; set; }
            public List<string>? Features { get; set; }
            public decimal? PricePerDay { get; set; }
            public decimal? PricePerDayUsd { get; set; }
            public decimal? PricePerDayInr { get; set; }
            public decimal? PricePerKm { get; set; }
            public decimal? PricePerKmUsd { get; set; }
            public decimal? PricePerKmInr { get; set; }
            public string? DriverName { get; set; }
            public string? DriverContact { get; set; }
            public string? InsuranceNumber { get; set; }
            public DateOnly? InsuranceExpiry { get; set; }
            public string? PermitNumber { get; set; }
            public DateOnly? PermitExpiry { get; set; }
            public string? Description { get; set; }
            public bool? IsActive { get; set; }
        }

        public class VehicleResponse
        {
            public long Id { get; set; }
            public string VehicleType { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public string RegistrationNumber { get; set; } = string.Empty;
            public int Capacity { get; set; }
            public List<string> Features { get; set; } = new();
            public decimal PricePerDay { get; set; }
            public decimal? PricePerDayUsd { get; set; }
            public decimal? PricePerDayInr { get; set; }
            public decimal? PricePerKm { get; set; }
            public decimal? PricePerKmUsd { get; set; }
            public decimal? PricePerKmInr { get; set; }
            public string? DriverName { get; set; }
            public string? DriverContact { get; set; }
            public string? InsuranceNumber { get; set; }
            public DateOnly? InsuranceExpiry { get; set; }
            public string? PermitNumber { get; set; }
            public DateOnly? PermitExpiry { get; set; }
            public string? Description { get; set; }
            public bool IsActive { get; set; }
            public int DaysBooked { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // GUIDES
        // ===========================
        public class CreateGuideRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Address { get; set; }
            public int ExperienceYears { get; set; }
            public List<string>? Languages { get; set; }
            public string? Specialization { get; set; }
            public string? CertificationNumber { get; set; }
            public DateTime? CertificationExpiry { get; set; }
            public string? LicenseNumber { get; set; }
            public decimal PricePerDay { get; set; }
            public decimal? PricePerDayUsd { get; set; }
            public decimal? PricePerDayInr { get; set; }
            public string? Bio { get; set; }
            public string? Photo { get; set; }
        }

        public class UpdateGuideRequest
        {
            public string? FullName { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Address { get; set; }
            public int? ExperienceYears { get; set; }
            public List<string>? Languages { get; set; }
            public string? Specialization { get; set; }
            public string? CertificationNumber { get; set; }
            public DateTime? CertificationExpiry { get; set; }
            public string? LicenseNumber { get; set; }
            public decimal? PricePerDay { get; set; }
            public decimal? PricePerDayUsd { get; set; }
            public decimal? PricePerDayInr { get; set; }
            public string? Bio { get; set; }
            public string? Photo { get; set; }
            public bool? IsActive { get; set; }
        }

        public class GuideResponse
        {
            public long Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Address { get; set; }
            public int ExperienceYears { get; set; }
            public List<string> Languages { get; set; } = new();
            public string? Specialization { get; set; }
            public string? CertificationNumber { get; set; }
            public DateTime? CertificationExpiry { get; set; }
            public string? LicenseNumber { get; set; }
            public decimal PricePerDay { get; set; }
            public decimal? PricePerDayUsd { get; set; }
            public decimal? PricePerDayInr { get; set; }
            public decimal? Rating { get; set; }
            public string? Bio { get; set; }
            public string? Photo { get; set; }
            public bool IsActive { get; set; }
            public int TripsCompleted { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // ACTIVITIES
        // ===========================
        public class CreateActivityRequest
        {
            public string Name { get; set; } = string.Empty;
            public string ActivityType { get; set; } = string.Empty;
            public string? Location { get; set; }
            public int DurationHours { get; set; }
            public string DifficultyLevel { get; set; } = "Moderate";
            public int MaxParticipants { get; set; }
            public int MinParticipants { get; set; } = 1;
            public List<string>? Equipment { get; set; }
            public decimal PricePerPerson { get; set; }
            public decimal? PricePerPersonUsd { get; set; }
            public decimal? PricePerPersonInr { get; set; }
            public string? Description { get; set; }
            public string? SafetyInstructions { get; set; }
            public List<string>? Images { get; set; }
        }

        public class UpdateActivityRequest
        {
            public string? Name { get; set; }
            public string? ActivityType { get; set; }
            public string? Location { get; set; }
            public int? DurationHours { get; set; }
            public string? DifficultyLevel { get; set; }
            public int? MaxParticipants { get; set; }
            public int? MinParticipants { get; set; }
            public List<string>? Equipment { get; set; }
            public decimal? PricePerPerson { get; set; }
            public decimal? PricePerPersonUsd { get; set; }
            public decimal? PricePerPersonInr { get; set; }
            public string? Description { get; set; }
            public string? SafetyInstructions { get; set; }
            public List<string>? Images { get; set; }
            public bool? IsActive { get; set; }
        }

        public class ActivityResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string ActivityType { get; set; } = string.Empty;
            public string? Location { get; set; }
            public int DurationHours { get; set; }
            public string DifficultyLevel { get; set; } = string.Empty;
            public int MaxParticipants { get; set; }
            public int MinParticipants { get; set; }
            public List<string> Equipment { get; set; } = new();
            public decimal PricePerPerson { get; set; }
            public decimal? PricePerPersonUsd { get; set; }
            public decimal? PricePerPersonInr { get; set; }
            public string? Description { get; set; }
            public string? SafetyInstructions { get; set; }
            public List<string> Images { get; set; } = new();
            public bool IsActive { get; set; }
            public int TotalBookings { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
