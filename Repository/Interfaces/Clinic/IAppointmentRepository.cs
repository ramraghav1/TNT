using System;
using System.Collections.Generic;
using static Repository.DataModels.Clinic.AppointmentDTO;

namespace Repository.Interfaces.Clinic
{
    public interface IAppointmentRepository
    {
        AppointmentResponse Create(CreateAppointmentRequest request);
        List<AppointmentResponse> GetByTenant(long tenantId);
        List<AppointmentResponse> GetByDateRange(long tenantId, DateOnly from, DateOnly to);
        List<AppointmentResponse> GetByPractitioner(long tenantId, long practitionerId, DateOnly? date = null);
        List<AppointmentResponse> GetByPatient(long tenantId, long patientId);
        AppointmentResponse? GetById(long tenantId, long id);
        AppointmentResponse? Update(long tenantId, long id, UpdateAppointmentRequest request);
        bool Delete(long tenantId, long id);
    }
}
