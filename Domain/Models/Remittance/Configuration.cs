using System;

namespace Domain.Models.Remittance
{
    public class Configuration
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreateConfigurationRequest
        {
            public long ConfigurationTypeId { get; set; }
            public string Code { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
        }

        public class UpdateConfigurationRequest
        {
            public string? Code { get; set; }
            public string? DisplayName { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class ConfigurationResponse
        {
            public long Id { get; set; }
            public long ConfigurationTypeId { get; set; }
            public string ConfigurationTypeName { get; set; } = string.Empty;
            public string Code { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
