using System;

namespace Domain.Models.Remittance
{
    public class AgentAccount
    {
        // ===========================
        // Create
        // ===========================
        public class CreateAgentAccountRequest
        {
            public long AgentId { get; set; }
            public string? AccountName { get; set; }
            public string? AccountNumber { get; set; }
            public string? BankName { get; set; }
            public string? BankBranch { get; set; }
            public string? BankDetails { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
            public decimal Balance { get; set; }
        }

        // ===========================
        // Update
        // ===========================
        public class UpdateAgentAccountRequest
        {
            public string? AccountName { get; set; }
            public string? AccountNumber { get; set; }
            public string? BankName { get; set; }
            public string? BankBranch { get; set; }
            public string? BankDetails { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class AgentAccountResponse
        {
            public long Id { get; set; }
            public long AgentId { get; set; }
            public string? AgentName { get; set; }
            public string? AccountName { get; set; }
            public string? AccountNumber { get; set; }
            public string? BankName { get; set; }
            public string? BankBranch { get; set; }
            public string? BankDetails { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
            public decimal Balance { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
