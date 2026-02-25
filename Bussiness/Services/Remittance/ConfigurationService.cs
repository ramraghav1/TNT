using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.Configuration;

namespace Bussiness.Services.Remittance
{
    public interface IConfigurationService
    {
        ConfigurationResponse Create(CreateConfigurationRequest request);
        List<ConfigurationResponse> GetAll();
        List<ConfigurationResponse> GetByTypeId(long configurationTypeId);
        ConfigurationResponse? GetById(long id);
        ConfigurationResponse? Update(long id, UpdateConfigurationRequest request);
        bool Delete(long id);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository _repository;
        private readonly IMapper _mapper;

        public ConfigurationService(IConfigurationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public ConfigurationResponse Create(CreateConfigurationRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.ConfigurationDTO.CreateConfigurationRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<ConfigurationResponse>(result);
        }

        public List<ConfigurationResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<ConfigurationResponse>>(result);
        }

        public List<ConfigurationResponse> GetByTypeId(long configurationTypeId)
        {
            var result = _repository.GetByTypeId(configurationTypeId);
            return _mapper.Map<List<ConfigurationResponse>>(result);
        }

        public ConfigurationResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<ConfigurationResponse>(result);
        }

        public ConfigurationResponse? Update(long id, UpdateConfigurationRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.ConfigurationDTO.UpdateConfigurationRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<ConfigurationResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
