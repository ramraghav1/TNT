using System;

namespace Repository.DataModels.Remittance
{
    public class ConfigurationTypeDTO
    {
        public class CreateConfigurationTypeRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        public class UpdateConfigurationTypeRequest
        {
            public string? Name { get; set; }
            public bool? IsActive { get; set; }
        }

        public class ConfigurationTypeResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
