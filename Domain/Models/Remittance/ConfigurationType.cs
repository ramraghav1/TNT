using System;

namespace Domain.Models.Remittance
{
    public class ConfigurationType
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreateConfigurationTypeRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        public class UpdateConfigurationTypeRequest
        {
            public string? Name { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class ConfigurationTypeResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
