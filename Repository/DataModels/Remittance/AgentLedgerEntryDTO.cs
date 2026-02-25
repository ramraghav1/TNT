using System;
using System.Collections.Generic;

namespace Repository.DataModels.Remittance
{
    public class AgentLedgerEntryDTO
    {
        public class CreateLedgerEntryRequest
        {
            public long AgentAccountId { get; set; }
            public string TransactionType { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string ReferenceType { get; set; } = string.Empty;
            public string? ReferenceId { get; set; }
            public string? Description { get; set; }
            public string? CreatedBy { get; set; }
        }

        public class LedgerEntryResponse
        {
            public long Id { get; set; }
            public long AgentAccountId { get; set; }
            public string TransactionType { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public decimal BalanceBefore { get; set; }
            public decimal BalanceAfter { get; set; }
            public string ReferenceType { get; set; } = string.Empty;
            public string? ReferenceId { get; set; }
            public string? Description { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? CreatedBy { get; set; }
        }

        public class StatementRequest
        {
            public long AgentAccountId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        public class StatementResponse
        {
            public long AgentAccountId { get; set; }
            public string? AgentName { get; set; }
            public string? AccountName { get; set; }
            public string? AccountNumber { get; set; }
            public string? CurrencyCode { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal ClosingBalance { get; set; }
            public decimal TotalDebit { get; set; }
            public decimal TotalCredit { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public List<LedgerEntryResponse> Entries { get; set; } = new();
        }
    }
}
