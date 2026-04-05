using System;

namespace Domain.Models
{
    /// <summary>
    /// Represents a tenant in the multi-tenant SaaS application
    /// </summary>
    public class Tenant
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty; // e.g., "company1" for company1.yourdomain.com
        public string? LogoUrl { get; set; }
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string Status { get; set; } = "Active"; // Active, Suspended, Inactive
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public TenantSettings Settings { get; set; } = new();
    }

    /// <summary>
    /// Tenant-specific configuration settings
    /// </summary>
    public class TenantSettings
    {
        public int MaxUsers { get; set; } = 10;
        public int MaxBookingsPerMonth { get; set; } = 100;
        public bool IsTrialAccount { get; set; } = false;
        public DateTime? TrialExpiresAt { get; set; }
        public string? CustomDomain { get; set; }
        public string TimeZone { get; set; } = "UTC";
        public string Currency { get; set; } = "USD";
        
        // Branding/Theme Settings
        public string? PrimaryColor { get; set; } = "#3B82F6"; // Default blue
        public string? SecondaryColor { get; set; } = "#10B981"; // Default green
        public string? AccentColor { get; set; } = "#F59E0B"; // Default amber
        public string? LogoUrl { get; set; }
        public string? FaviconUrl { get; set; }
        
        // Database Configuration
        public string DatabaseType { get; set; } = "Shared"; // Shared, Dedicated
        public string? DatabaseConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        
        // Contract/Subscription Information
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string? ContractType { get; set; } // Monthly, Yearly, Custom
        public decimal? MonthlyFee { get; set; }
        public string? BillingEmail { get; set; }
        public string? PaymentStatus { get; set; } // Active, Pending, Overdue, Cancelled
    }

    /// <summary>
    /// Represents available products/modules in the system
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // TourAndTravel, Clinic, Remittance
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Maps tenants to their subscribed products
    /// </summary>
    public class TenantProduct
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public int ProductId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ActivatedAt { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string? SubscriptionTier { get; set; } // Basic, Premium, Enterprise
    }

    /// <summary>
    /// Request/Response DTOs for Tenant operations
    /// </summary>
    public static class TenantDto
    {
        public class CreateTenantRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Subdomain { get; set; } = string.Empty;
            public string ContactEmail { get; set; } = string.Empty;
            public string? ContactPhone { get; set; }
            public int[] ProductIds { get; set; } = Array.Empty<int>();
        }

        public class TenantResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Subdomain { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public TenantSettings Settings { get; set; } = new();
            public string[] Products { get; set; } = Array.Empty<string>();
        }
    }
}
