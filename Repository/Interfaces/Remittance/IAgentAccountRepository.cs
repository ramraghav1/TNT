using System.Collections.Generic;
using static Repository.DataModels.Remittance.AgentAccountDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IAgentAccountRepository
    {
        AgentAccountResponse Create(CreateAgentAccountRequest request);
        List<AgentAccountResponse> GetAll();
        List<AgentAccountResponse> GetByAgent(long agentId);
        AgentAccountResponse? GetById(long id);
        AgentAccountResponse? GetByAgentAndCurrency(long agentId, string currencyCode);
        AgentAccountResponse? Update(long id, UpdateAgentAccountRequest request);
        bool UpdateBalance(long id, decimal newBalance);
        bool Delete(long id);
    }
}
