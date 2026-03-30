using System.Collections.Generic;
using static Repository.DataModels.Clinic.PractitionerDTO;

namespace Repository.Interfaces.Clinic
{
    public interface IPractitionerRepository
    {
        PractitionerResponse Create(CreatePractitionerRequest request);
        List<PractitionerResponse> GetByTenant(long tenantId);
        PractitionerResponse? GetById(long tenantId, long id);
        PractitionerResponse? Update(long tenantId, long id, UpdatePractitionerRequest request);
        bool Delete(long tenantId, long id);
    }
}
