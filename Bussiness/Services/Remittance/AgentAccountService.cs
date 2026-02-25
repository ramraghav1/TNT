using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.AgentAccount;

namespace Bussiness.Services.Remittance
{
    public interface IAgentAccountService
    {
        AgentAccountResponse Create(CreateAgentAccountRequest request);
        List<AgentAccountResponse> GetAll();
        List<AgentAccountResponse> GetByAgent(long agentId);
        AgentAccountResponse? GetById(long id);
        AgentAccountResponse? GetByAgentAndCurrency(long agentId, string currencyCode);
        AgentAccountResponse? Update(long id, UpdateAgentAccountRequest request);
        bool Delete(long id);
    }

    public class AgentAccountService : IAgentAccountService
    {
        private readonly IAgentAccountRepository _repository;
        private readonly IMapper _mapper;

        public AgentAccountService(IAgentAccountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public AgentAccountResponse Create(CreateAgentAccountRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.AgentAccountDTO.CreateAgentAccountRequest>(request);
            var result = _repository.Create(dto);
            return _mapper.Map<AgentAccountResponse>(result);
        }

        public List<AgentAccountResponse> GetAll()
        {
            var result = _repository.GetAll();
            return _mapper.Map<List<AgentAccountResponse>>(result);
        }

        public List<AgentAccountResponse> GetByAgent(long agentId)
        {
            var result = _repository.GetByAgent(agentId);
            return _mapper.Map<List<AgentAccountResponse>>(result);
        }

        public AgentAccountResponse? GetById(long id)
        {
            var result = _repository.GetById(id);
            return result == null ? null : _mapper.Map<AgentAccountResponse>(result);
        }

        public AgentAccountResponse? GetByAgentAndCurrency(long agentId, string currencyCode)
        {
            var result = _repository.GetByAgentAndCurrency(agentId, currencyCode);
            return result == null ? null : _mapper.Map<AgentAccountResponse>(result);
        }

        public AgentAccountResponse? Update(long id, UpdateAgentAccountRequest request)
        {
            var dto = _mapper.Map<Repository.DataModels.Remittance.AgentAccountDTO.UpdateAgentAccountRequest>(request);
            var result = _repository.Update(id, dto);
            return result == null ? null : _mapper.Map<AgentAccountResponse>(result);
        }

        public bool Delete(long id)
        {
            return _repository.Delete(id);
        }
    }
}
