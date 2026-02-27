using System;
using System.Collections.Generic;

namespace Repository.DataModels.Remittance
{
    public class VoucherDTO
    {
        // ── Create Request ──
        public class CreateVoucherRequest
        {
            public string EntityType { get; set; } = string.Empty;
            public long? AgentId { get; set; }
            public long? BranchId { get; set; }
            public long? AgentAccountId { get; set; }
            public decimal Amount { get; set; }
            public string Mode { get; set; } = string.Empty;
            public string ReferenceType { get; set; } = string.Empty;
            public string? ReferenceId { get; set; }
            public string? Description { get; set; }
            public string? CreatedBy { get; set; }
        }

        // ── Response ──
        public class VoucherResponse
        {
            public long Id { get; set; }
            public string VoucherNumber { get; set; } = string.Empty;
            public DateTime VoucherDate { get; set; }
            public string EntityType { get; set; } = string.Empty;
            public long? AgentId { get; set; }
            public string? AgentName { get; set; }
            public long? BranchId { get; set; }
            public string? BranchName { get; set; }
            public long? AgentAccountId { get; set; }
            public decimal Amount { get; set; }
            public string Mode { get; set; } = string.Empty;
            public decimal BalanceBefore { get; set; }
            public decimal BalanceAfter { get; set; }
            public string ReferenceType { get; set; } = string.Empty;
            public string? ReferenceId { get; set; }
            public string? Description { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? CreatedBy { get; set; }
        }

        // ── Statement Request ──
        public class AccountStatementRequest
        {
            public string EntityType { get; set; } = string.Empty;
            public long? AgentId { get; set; }
            public long? BranchId { get; set; }
            public long? AgentAccountId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        // ── Statement Response ──
        public class AccountStatementResponse
        {
            public string EntityType { get; set; } = string.Empty;
            public long? AgentId { get; set; }
            public string? AgentName { get; set; }
            public long? BranchId { get; set; }
            public string? BranchName { get; set; }
            public long? AgentAccountId { get; set; }
            public string? CurrencyCode { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal ClosingBalance { get; set; }
            public decimal TotalCredit { get; set; }
            public decimal TotalDebit { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public List<VoucherResponse> Entries { get; set; } = new();
            public List<DailySummary> DailySummaries { get; set; } = new();
        }

        // ── Daily Summary ──
        public class DailySummary
        {
            public DateTime Date { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal TotalCredit { get; set; }
            public decimal TotalDebit { get; set; }
            public decimal ClosingBalance { get; set; }
            public int TransactionCount { get; set; }
        }
    }
}
