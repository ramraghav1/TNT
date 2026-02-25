using System.Collections.Generic;
using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.ConfigurationType;

namespace Bussiness.Services.Remittance
{
    public interface IConfigurationTypeService
    {
        ConfigurationTypeResponse Create(CreateConfigurationTypeRequest request);
        List<ConfigurationTypeResponse> GetAll();
        ConfigurationTypeResponse? GetById(long id);
        ConfigurationTypeResponse? Update(long id, UpdateConfigurationTypeRequest request);
        bool Delete(long id);
    }

    public class ConfigurationTypeService : IConfigurationTypeService
    {
        private readonly IConfigurationTypeRepository _repository;
        private readonly IMapper _mapper;

        public ConfigurationTypeService(IConfigurationTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public ConfigurationTypeResponse Create(CreateConfigurationTypeRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.ConfigurationTypeDTO.CreateConfigurationTypeRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<ConfigurationTypeResponse>(result);
        }

        public List<ConfigurationTypeResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<ConfigurationTypeResponse>>(result);
        }

        public ConfigurationTypeResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<ConfigurationTypeResponse>(result);
        }

        public ConfigurationTypeResponse? Update(long id, UpdateConfigurationTypeRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.ConfigurationTypeDTO.UpdateConfigurationTypeRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<ConfigurationTypeResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
