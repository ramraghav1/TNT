using System;

namespace Domain.Models.Clinic
{
    public class ClinicService
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreateClinicServiceRequest
        {
            public long TenantId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int DurationMinutes { get; set; } = 30;
            public decimal Price { get; set; }
            public string Currency { get; set; } = "NPR";
            public string? Category { get; set; }
        }

        public class UpdateClinicServiceRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? DurationMinutes { get; set; }
            public decimal? Price { get; set; }
            public string? Currency { get; set; }
            public string? Category { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class ClinicServiceResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int DurationMinutes { get; set; }
            public decimal Price { get; set; }
            public string Currency { get; set; } = "NPR";
            public string? Category { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
