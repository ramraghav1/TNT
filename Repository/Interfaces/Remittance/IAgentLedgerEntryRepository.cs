using System;
using System.Collections.Generic;
using static Repository.DataModels.Remittance.AgentLedgerEntryDTO;

namespace Repository.Interfaces.Remittance
{
    public interface IAgentLedgerEntryRepository
    {
        LedgerEntryResponse Create(CreateLedgerEntryRequest request, decimal balanceBefore, decimal balanceAfter);
        List<LedgerEntryResponse> GetByAccount(long agentAccountId);
        List<LedgerEntryResponse> GetByAccountAndDateRange(long agentAccountId, DateTime? fromDate, DateTime? toDate);
        LedgerEntryResponse? GetById(long id);
    }
}
