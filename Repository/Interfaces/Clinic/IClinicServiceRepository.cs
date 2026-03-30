using System.Collections.Generic;
using static Repository.DataModels.Clinic.ClinicServiceDTO;

namespace Repository.Interfaces.Clinic
{
    public interface IClinicServiceRepository
    {
        ClinicServiceResponse Create(CreateClinicServiceRequest request);
        List<ClinicServiceResponse> GetByTenant(long tenantId);
        ClinicServiceResponse? GetById(long tenantId, long id);
        ClinicServiceResponse? Update(long tenantId, long id, UpdateClinicServiceRequest request);
        bool Delete(long tenantId, long id);
    }
}
