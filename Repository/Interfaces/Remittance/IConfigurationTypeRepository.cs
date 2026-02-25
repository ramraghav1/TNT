using System.Collections.Generic;
using static Repository.DataModels.Remittance.ConfigurationTypeDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IConfigurationTypeRepository
    {
        ConfigurationTypeResponse Create(CreateConfigurationTypeRequest request);
        List<ConfigurationTypeResponse> GetAll();
        ConfigurationTypeResponse? GetById(long id);
        ConfigurationTypeResponse? Update(long id, UpdateConfigurationTypeRequest request);
        bool Delete(long id);
    }
}
