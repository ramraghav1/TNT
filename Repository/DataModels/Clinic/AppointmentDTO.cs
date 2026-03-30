using System;

namespace Repository.DataModels.Clinic
{
    public class AppointmentDTO
    {
        public class CreateAppointmentRequest
        {
            public long TenantId { get; set; }
            public long PatientId { get; set; }
            public long PractitionerId { get; set; }
            public long ServiceId { get; set; }
            public DateOnly AppointmentDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            public string Status { get; set; } = "Scheduled";
            public string? Notes { get; set; }
            public string? RecurrenceRule { get; set; }
            public Guid? RecurrenceGroupId { get; set; }
        }

        public class UpdateAppointmentRequest
        {
            public long? PatientId { get; set; }
            public long? PractitionerId { get; set; }
            public long? ServiceId { get; set; }
            public DateOnly? AppointmentDate { get; set; }
            public TimeOnly? StartTime { get; set; }
            public TimeOnly? EndTime { get; set; }
            public string? Status { get; set; }
            public string? Notes { get; set; }
            public string? RecurrenceRule { get; set; }
        }

        public class AppointmentResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public long PatientId { get; set; }
            public string? PatientName { get; set; }
            public long PractitionerId { get; set; }
            public string? PractitionerName { get; set; }
            public long ServiceId { get; set; }
            public string? ServiceName { get; set; }
            public DateOnly AppointmentDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
            public string Status { get; set; } = "Scheduled";
            public string? Notes { get; set; }
            public string? RecurrenceRule { get; set; }
            public Guid? RecurrenceGroupId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
