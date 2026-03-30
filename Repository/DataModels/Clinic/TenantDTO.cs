using System;

namespace Repository.DataModels.Clinic
{
    public class TenantDTO
    {
        public class CreateTenantRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? LogoUrl { get; set; }
            public string Timezone { get; set; } = "Asia/Kathmandu";
        }

        public class UpdateTenantRequest
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? LogoUrl { get; set; }
            public string? Timezone { get; set; }
            public bool? IsActive { get; set; }
        }

        public class TenantResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
            public string? LogoUrl { get; set; }
            public string Timezone { get; set; } = "Asia/Kathmandu";
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
