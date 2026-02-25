using System.Collections.Generic;
using static Repository.DataModels.Remittance.AdministrativeDivisionDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IAdministrativeDivisionRepository
    {
        List<AdministrativeDivisionResponse> GetByCountry(long countryId);
        List<AdministrativeDivisionResponse> GetByCountryAndLevel(long countryId, int level);
        List<AdministrativeDivisionResponse> GetChildren(long parentId);
    }
}
