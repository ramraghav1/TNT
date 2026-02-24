using System.Collections.Generic;
using static Repository.DataModels.Remittance.AgentDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IAgentRepository
    {
        AgentResponse Create(CreateAgentRequest request);
        List<AgentResponse> GetAll();
        List<AgentResponse> GetByCountry(long countryId);
        AgentResponse? GetById(long id);
        AgentResponse? Update(long id, UpdateAgentRequest request);
        bool Delete(long id);
    }
}
