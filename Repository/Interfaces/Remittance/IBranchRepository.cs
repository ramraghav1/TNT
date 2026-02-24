using System.Collections.Generic;
using static Repository.DataModels.Remittance.BranchDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IBranchRepository
    {
        BranchResponse Create(CreateBranchRequest request);
        List<BranchResponse> GetAll();
        List<BranchResponse> GetByAgent(long agentId);
        BranchResponse? GetById(long id);
        BranchResponse? Update(long id, UpdateBranchRequest request);
        bool Delete(long id);
    }
}
