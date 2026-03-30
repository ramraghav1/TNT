using System;

namespace Repository.DataModels.Clinic
{
    public class PractitionerDTO
    {
        public class CreatePractitionerRequest
        {
            public long TenantId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Specialization { get; set; }
            public string? Bio { get; set; }
            public string Role { get; set; } = "Practitioner";
        }

        public class UpdatePractitionerRequest
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Specialization { get; set; }
            public string? Bio { get; set; }
            public string? Role { get; set; }
            public bool? IsActive { get; set; }
        }

        public class PractitionerResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Specialization { get; set; }
            public string? Bio { get; set; }
            public string Role { get; set; } = "Practitioner";
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
