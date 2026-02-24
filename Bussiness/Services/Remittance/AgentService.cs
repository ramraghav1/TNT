using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.Agent;

namespace Bussiness.Services.Remittance
{
    public interface IAgentService
    {
        AgentResponse Create(CreateAgentRequest request);
        List<AgentResponse> GetAll();
        List<AgentResponse> GetByCountry(long countryId);
        AgentResponse? GetById(long id);
        AgentResponse? Update(long id, UpdateAgentRequest request);
        bool Delete(long id);
    }

    public class AgentService : IAgentService
    {
        private readonly IAgentRepository _repository;
        private readonly IMapper _mapper;

        public AgentService(IAgentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public AgentResponse Create(CreateAgentRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.AgentDTO.CreateAgentRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<AgentResponse>(result);
        }

        public List<AgentResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<AgentResponse>>(result);
        }

        public List<AgentResponse> GetByCountry(long countryId)
        {
            var result = _repository.GetByCountry(countryId);
            return _mapper.Map<List<AgentResponse>>(result);
        }

        public AgentResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<AgentResponse>(result);
        }

        public AgentResponse? Update(long id, UpdateAgentRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.AgentDTO.UpdateAgentRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<AgentResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
