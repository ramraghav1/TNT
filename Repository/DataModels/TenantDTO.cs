using System;

namespace Repository.DataModels
{
    public class TenantDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string Status { get; set; } = string.Empty;
        public string SettingsJson { get; set; } = "{}"; // Stored as JSONB
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class TenantProductDTO
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public int ProductId { get; set; }
        public bool IsActive { get; set; }
        public DateTime ActivatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string? SubscriptionTier { get; set; }
    }
}
