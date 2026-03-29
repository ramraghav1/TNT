using System.Collections.Generic;
using static Repository.DataModels.Clinic.TenantDTO;

namespace Repository.Interfaces.Clinic
{
    public interface ITenantRepository
    {
        TenantResponse Create(CreateTenantRequest request);
        List<TenantResponse> GetAll();
        TenantResponse? GetById(long id);
        TenantResponse? GetBySlug(string slug);
        TenantResponse? Update(long id, UpdateTenantRequest request);
        bool Delete(long id);
    }
}
