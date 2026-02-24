using System.Collections.Generic;
using static Repository.DataModels.Remittance.BranchUserDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IBranchUserRepository
    {
        BranchUserResponse Create(CreateBranchUserRequest request);
        List<BranchUserResponse> GetByBranch(long branchId);
        BranchUserResponse? GetById(long id);
        BranchUserResponse? Update(long id, UpdateBranchUserRequest request);
        bool Delete(long id);
    }
}
