using System;

namespace Domain.Models.Clinic
{
    public class Patient
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreatePatientRequest
        {
            public long TenantId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Gender { get; set; }
            public string? Address { get; set; }
            public string? MedicalNotes { get; set; }
            public string? Allergies { get; set; }
            public string? EmergencyContactName { get; set; }
            public string? EmergencyContactPhone { get; set; }
        }

        public class UpdatePatientRequest
        {
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Gender { get; set; }
            public string? Address { get; set; }
            public string? MedicalNotes { get; set; }
            public string? Allergies { get; set; }
            public string? EmergencyContactName { get; set; }
            public string? EmergencyContactPhone { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class PatientResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public DateOnly? DateOfBirth { get; set; }
            public string? Gender { get; set; }
            public string? Address { get; set; }
            public string? MedicalNotes { get; set; }
            public string? Allergies { get; set; }
            public string? EmergencyContactName { get; set; }
            public string? EmergencyContactPhone { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
