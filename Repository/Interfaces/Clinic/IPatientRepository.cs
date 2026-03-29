using System.Collections.Generic;
using static Repository.DataModels.Clinic.PatientDTO;

namespace Repository.Interfaces.Clinic
{
    public interface IPatientRepository
    {
        PatientResponse Create(CreatePatientRequest request);
        List<PatientResponse> GetByTenant(long tenantId);
        PatientResponse? GetById(long tenantId, long id);
        PatientResponse? Update(long tenantId, long id, UpdatePatientRequest request);
        bool Delete(long tenantId, long id);
    }
}
