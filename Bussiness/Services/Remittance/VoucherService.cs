using AutoMapper;
using Repository.Interfaces.Remittance;
using static Domain.Models.Remittance.Voucher;

namespace Bussiness.Services.Remittance
{
    public interface IVoucherService
    {
        /// <summary>
        /// Post a voucher entry. Automatically updates agent/branch balance.
        /// CR adds balance, DR deducts balance.
        /// </summary>
        VoucherResponse PostVoucher(CreateVoucherRequest request);

        /// <summary>
        /// Get all vouchers.
        /// </summary>
        List<VoucherResponse> GetAll();

        /// <summary>
        /// Get voucher by ID.
        /// </summary>
        VoucherResponse? GetById(long id);

        /// <summary>
        /// Get vouchers by agent.
        /// </summary>
        List<VoucherResponse> GetByAgent(long agentId);

        /// <summary>
        /// Get vouchers by branch.
        /// </summary>
        List<VoucherResponse> GetByBranch(long branchId);

        /// <summary>
        /// Get statement of account with opening/closing balance, daily summaries.
        /// </summary>
        AccountStatementResponse GetStatement(AccountStatementRequest request);
    }

    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly IAgentAccountRepository _accountRepo;
        private readonly IMapper _mapper;

        public VoucherService(
            IVoucherRepository voucherRepo,
            IAgentAccountRepository accountRepo,
            IMapper mapper)
        {
            _voucherRepo = voucherRepo;
            _accountRepo = accountRepo;
            _mapper = mapper;
        }

        public VoucherResponse PostVoucher(CreateVoucherRequest request)
        {
            // Validate
            if (request.Mode?.ToUpper() != "CR" && request.Mode?.ToUpper() != "DR")
                throw new Exception("Mode must be 'CR' or 'DR'.");

            if (request.EntityType?.ToUpper() != "AGENT" && request.EntityType?.ToUpper() != "BRANCH")
                throw new Exception("EntityType must be 'AGENT' or 'BRANCH'.");

            request.Mode = request.Mode!.ToUpper();
            request.EntityType = request.EntityType!.ToUpper();

            decimal balanceBefore;
            decimal balanceAfter;

            if (request.EntityType == "AGENT")
            {
                // Use agent account balance
                if (!request.AgentAccountId.HasValue)
                    throw new Exception("AgentAccountId is required for AGENT vouchers.");

                var account = _accountRepo.GetById(request.AgentAccountId.Value)
                    ?? throw new Exception($"Agent account {request.AgentAccountId} not found.");

                balanceBefore = account.Balance;
                balanceAfter = request.Mode == "CR"
                    ? balanceBefore + request.Amount
                    : balanceBefore - request.Amount;

                // Generate voucher number
                string voucherNumber = $"V-A-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

                // Create voucher entry
                var dto = _mapper.Map<Repository.DataModels.Remittance.VoucherDTO.CreateVoucherRequest>(request);
                var entry = _voucherRepo.Create(dto, balanceBefore, balanceAfter, voucherNumber);

                // Update agent account balance
                _accountRepo.UpdateBalance(request.AgentAccountId.Value, balanceAfter);

                return _mapper.Map<VoucherResponse>(entry);
            }
            else // BRANCH
            {
                if (!request.BranchId.HasValue)
                    throw new Exception("BranchId is required for BRANCH vouchers.");

                balanceBefore = _voucherRepo.GetBranchBalance(request.BranchId.Value);
                balanceAfter = request.Mode == "CR"
                    ? balanceBefore + request.Amount
                    : balanceBefore - request.Amount;

                // Generate voucher number
                string voucherNumber = $"V-B-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

                // Create voucher entry
                var dto = _mapper.Map<Repository.DataModels.Remittance.VoucherDTO.CreateVoucherRequest>(request);
                var entry = _voucherRepo.Create(dto, balanceBefore, balanceAfter, voucherNumber);

                // Update branch balance
                _voucherRepo.UpdateBranchBalance(request.BranchId.Value, balanceAfter);

                return _mapper.Map<VoucherResponse>(entry);
            }
        }

        public List<VoucherResponse> GetAll()
        {
            var result = _voucherRepo.GetAll();
            return _mapper.Map<List<VoucherResponse>>(result);
        }

        public VoucherResponse? GetById(long id)
        {
            var result = _voucherRepo.GetById(id);
            return result == null ? null : _mapper.Map<VoucherResponse>(result);
        }

        public List<VoucherResponse> GetByAgent(long agentId)
        {
            var result = _voucherRepo.GetByAgent(agentId);
            return _mapper.Map<List<VoucherResponse>>(result);
        }

        public List<VoucherResponse> GetByBranch(long branchId)
        {
            var result = _voucherRepo.GetByBranch(branchId);
            return _mapper.Map<List<VoucherResponse>>(result);
        }

        public AccountStatementResponse GetStatement(AccountStatementRequest request)
        {
            var entries = _voucherRepo.GetByEntityAndDateRange(
                request.EntityType, request.AgentId, request.BranchId,
                request.AgentAccountId, request.FromDate, request.ToDate);

            var mappedEntries = _mapper.Map<List<VoucherResponse>>(entries);

            // Calculate opening/closing
            decimal openingBalance = mappedEntries.Count > 0
                ? mappedEntries[0].BalanceBefore
                : GetCurrentBalance(request);
            decimal closingBalance = mappedEntries.Count > 0
                ? mappedEntries[^1].BalanceAfter
                : GetCurrentBalance(request);
            decimal totalCredit = mappedEntries.Where(e => e.Mode == "CR").Sum(e => e.Amount);
            decimal totalDebit = mappedEntries.Where(e => e.Mode == "DR").Sum(e => e.Amount);

            // Build daily summaries
            var dailySummaries = BuildDailySummaries(mappedEntries, openingBalance);

            // Get entity names
            string? agentName = null;
            string? branchName = null;
            string? currencyCode = null;

            if (mappedEntries.Count > 0)
            {
                agentName = mappedEntries[0].AgentName;
                branchName = mappedEntries[0].BranchName;
            }

            if (request.AgentAccountId.HasValue)
            {
                var account = _accountRepo.GetById(request.AgentAccountId.Value);
                if (account != null)
                {
                    agentName = account.AgentName;
                    currencyCode = account.CurrencyCode;
                }
            }

            return new AccountStatementResponse
            {
                EntityType = request.EntityType,
                AgentId = request.AgentId,
                AgentName = agentName,
                BranchId = request.BranchId,
                BranchName = branchName,
                AgentAccountId = request.AgentAccountId,
                CurrencyCode = currencyCode,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                TotalCredit = totalCredit,
                TotalDebit = totalDebit,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Entries = mappedEntries,
                DailySummaries = dailySummaries
            };
        }

        private decimal GetCurrentBalance(AccountStatementRequest request)
        {
            if (request.EntityType?.ToUpper() == "AGENT" && request.AgentAccountId.HasValue)
            {
                var account = _accountRepo.GetById(request.AgentAccountId.Value);
                return account?.Balance ?? 0;
            }
            else if (request.EntityType?.ToUpper() == "BRANCH" && request.BranchId.HasValue)
            {
                return _voucherRepo.GetBranchBalance(request.BranchId.Value);
            }
            return 0;
        }

        private List<DailySummary> BuildDailySummaries(List<VoucherResponse> entries, decimal openingBalance)
        {
            var summaries = new List<DailySummary>();
            if (entries.Count == 0) return summaries;

            var grouped = entries.GroupBy(e => e.VoucherDate.Date).OrderBy(g => g.Key);
            decimal runningBalance = openingBalance;

            foreach (var group in grouped)
            {
                decimal dayCredit = group.Where(e => e.Mode == "CR").Sum(e => e.Amount);
                decimal dayDebit = group.Where(e => e.Mode == "DR").Sum(e => e.Amount);
                decimal dayClosing = runningBalance + dayCredit - dayDebit;

                summaries.Add(new DailySummary
                {
                    Date = group.Key,
                    OpeningBalance = runningBalance,
                    TotalCredit = dayCredit,
                    TotalDebit = dayDebit,
                    ClosingBalance = dayClosing,
                    TransactionCount = group.Count()
                });

                runningBalance = dayClosing;
            }

            return summaries;
        }
    }
}
