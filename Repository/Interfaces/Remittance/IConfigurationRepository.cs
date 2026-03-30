using System.Collections.Generic;
using static Repository.DataModels.Remittance.ConfigurationDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IConfigurationRepository
    {
        ConfigurationResponse Create(CreateConfigurationRequest request);
        List<ConfigurationResponse> GetAll();
        List<ConfigurationResponse> GetByTypeId(long configurationTypeId);
        List<ConfigurationResponse> GetByTypeName(string typeName);
        ConfigurationResponse? GetById(long id);
        ConfigurationResponse? Update(long id, UpdateConfigurationRequest request);
        bool Delete(long id);
    }
}
