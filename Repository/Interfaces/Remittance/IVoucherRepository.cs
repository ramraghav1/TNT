using System;
using System.Collections.Generic;
using static Repository.DataModels.Remittance.VoucherDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IVoucherRepository
    {
        VoucherResponse Create(CreateVoucherRequest request, decimal balanceBefore, decimal balanceAfter, string voucherNumber);
        List<VoucherResponse> GetAll();
        VoucherResponse? GetById(long id);
        List<VoucherResponse> GetByAgent(long agentId);
        List<VoucherResponse> GetByBranch(long branchId);
        List<VoucherResponse> GetByAgentAccount(long agentAccountId);
        List<VoucherResponse> GetByEntityAndDateRange(string entityType, long? agentId, long? branchId, long? agentAccountId, DateTime? fromDate, DateTime? toDate);
        decimal GetBranchBalance(long branchId);
        bool UpdateBranchBalance(long branchId, decimal newBalance);
    }
}
