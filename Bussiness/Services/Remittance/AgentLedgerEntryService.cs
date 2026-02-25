using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.AgentLedgerEntry;

namespace Bussiness.Services.Remittance
{
    public interface IAgentLedgerEntryService
    {
        /// <summary>
        /// Post a ledger entry with automatic balance update (DEBIT subtracts, CREDIT adds).
        /// </summary>
        LedgerEntryResponse PostEntry(CreateLedgerEntryRequest request);

        /// <summary>
        /// Get all ledger entries for an account.
        /// </summary>
        List<LedgerEntryResponse> GetByAccount(long agentAccountId);

        /// <summary>
        /// Get statement of account with opening/closing balance and totals.
        /// </summary>
        StatementResponse GetStatement(StatementRequest request);
    }

    public class AgentLedgerEntryService : IAgentLedgerEntryService
    {
        private readonly IAgentLedgerEntryRepository _ledgerRepo;
        private readonly IAgentAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public AgentLedgerEntryService(
            IAgentLedgerEntryRepository ledgerRepo,
            IAgentAccountRepository accountRepo,
            IMapper mapper)
        {
            _ledgerRepo = ledgerRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
        }

        public LedgerEntryResponse PostEntry(CreateLedgerEntryRequest request)
        {
            // Get current account balance
            var account = _accountRepo.GetById(request.AgentAccountId)
                ?? throw new Exception($"Agent account {request.AgentAccountId} not found.");

            decimal balanceBefore = account.Balance;
            decimal balanceAfter;

            if (request.TransactionType.ToUpper() == "DEBIT")
                balanceAfter = balanceBefore - request.Amount;
            else if (request.TransactionType.ToUpper() == "CREDIT")
                balanceAfter = balanceBefore + request.Amount;
            else
                throw new Exception($"Invalid transaction type: {request.TransactionType}. Must be DEBIT or CREDIT.");

            // Map and create ledger entry
            var dto = _mapper.Map<Repository.DataModels.Remittance.AgentLedgerEntryDTO.CreateLedgerEntryRequest>(request);
            var entry = _ledgerRepo.Create(dto, balanceBefore, balanceAfter);

            // Update account balance
            _accountRepo.UpdateBalance(request.AgentAccountId, balanceAfter);

            return _mapper.Map<LedgerEntryResponse>(entry);
        }

        public List<LedgerEntryResponse> GetByAccount(long agentAccountId)
        {
            var result = _ledgerRepo.GetByAccount(agentAccountId);
            return _mapper.Map<List<LedgerEntryResponse>>(result);
        }

        public StatementResponse GetStatement(StatementRequest request)
        {
            // Get account info
            var account = _accountRepo.GetById(request.AgentAccountId)
                ?? throw new Exception($"Agent account {request.AgentAccountId} not found.");

            // Get entries for the date range
            var dtoEntries = _ledgerRepo.GetByAccountAndDateRange(
                request.AgentAccountId, request.FromDate, request.ToDate);

            var entries = _mapper.Map<List<LedgerEntryResponse>>(dtoEntries);

            // Calculate opening balance (balance_before of first entry, or current balance if no entries)
            decimal openingBalance = entries.Count > 0 ? entries[0].BalanceBefore : account.Balance;
            decimal closingBalance = entries.Count > 0 ? entries[^1].BalanceAfter : account.Balance;
            decimal totalDebit = entries.Where(e => e.TransactionType == "DEBIT").Sum(e => e.Amount);
            decimal totalCredit = entries.Where(e => e.TransactionType == "CREDIT").Sum(e => e.Amount);

            return new StatementResponse
            {
                AgentAccountId = account.Id,
                AgentName = account.AgentName,
                AccountName = account.AccountName,
                AccountNumber = account.AccountNumber,
                CurrencyCode = account.CurrencyCode,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                TotalDebit = totalDebit,
                TotalCredit = totalCredit,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Entries = entries
            };
        }
    }
}
