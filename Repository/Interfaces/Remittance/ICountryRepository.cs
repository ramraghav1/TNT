using System.Collections.Generic;
using static Repository.DataModels.Remittance.CountryDTO;

namespace Repository.Interfaces.Remittance
{
    public interface ICountryRepository
    {
        CountryResponse Create(CreateCountryRequest request);
        List<CountryResponse> GetAll();
        CountryResponse? GetById(long id);
        CountryResponse? Update(long id, UpdateCountryRequest request);
        bool Delete(long id);
    }
}
