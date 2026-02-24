using System;
using System.Collections.Generic;

namespace Repository.DataModels.Remittance
{
    public class FxRateSetupDTO
    {
        // ===========================
        // Create
        // ===========================
        public class CreateFxRateRequest
        {
            public long SendingCountryId { get; set; }
            public long ReceivingCountryId { get; set; }
            public long PaymentTypeId { get; set; }
            public long? AgentId { get; set; }
            public string SendingCurrency { get; set; } = string.Empty;
            public string ReceivingCurrency { get; set; } = string.Empty;
            public string? SettlementCurrency { get; set; }
            public decimal CustomerRate { get; set; }
            public decimal? SettlementRate { get; set; }
            public decimal? CrossRate { get; set; }
            public string? MarginType { get; set; }
            public decimal? MarginValue { get; set; }
            public DateTime? ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public string? CreatedBy { get; set; }
        }

        // ===========================
        // Update
        // ===========================
        public class UpdateFxRateRequest
        {
            public string? SendingCurrency { get; set; }
            public string? ReceivingCurrency { get; set; }
            public string? SettlementCurrency { get; set; }
            public decimal? CustomerRate { get; set; }
            public decimal? SettlementRate { get; set; }
            public decimal? CrossRate { get; set; }
            public string? MarginType { get; set; }
            public decimal? MarginValue { get; set; }
            public DateTime? ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public bool? IsActive { get; set; }
            public string? UpdatedBy { get; set; }
        }

        // ===========================
        // Response (list item)
        // ===========================
        public class FxRateListItem
        {
            public long Id { get; set; }
            public long SendingCountryId { get; set; }
            public string SendingCountryName { get; set; } = string.Empty;
            public long ReceivingCountryId { get; set; }
            public string ReceivingCountryName { get; set; } = string.Empty;
            public long PaymentTypeId { get; set; }
            public string PaymentTypeName { get; set; } = string.Empty;
            public long? AgentId { get; set; }
            public string? AgentName { get; set; }
            public string SendingCurrency { get; set; } = string.Empty;
            public string ReceivingCurrency { get; set; } = string.Empty;
            public string? SettlementCurrency { get; set; }
            public decimal CustomerRate { get; set; }
            public decimal? SettlementRate { get; set; }
            public decimal? CrossRate { get; set; }
            public string? MarginType { get; set; }
            public decimal? MarginValue { get; set; }
            public DateTime? ValidFrom { get; set; }
            public DateTime? ValidTo { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        // ===========================
        // Detail (with history)
        // ===========================
        public class FxRateDetailResponse : FxRateListItem
        {
            public List<FxRateHistoryItem> History { get; set; } = new();
        }

        // ===========================
        // Rate History
        // ===========================
        public class FxRateHistoryItem
        {
            public long Id { get; set; }
            public decimal? PreviousCustomerRate { get; set; }
            public decimal NewCustomerRate { get; set; }
            public decimal? PreviousSettlementRate { get; set; }
            public decimal? NewSettlementRate { get; set; }
            public decimal? PreviousCrossRate { get; set; }
            public decimal? NewCrossRate { get; set; }
            public string? ChangedBy { get; set; }
            public DateTime ChangedAt { get; set; }
            public string? Reason { get; set; }
        }

        // ===========================
        // Convert
        // ===========================
        public class ConvertRequest
        {
            public long SendingCountryId { get; set; }
            public long ReceivingCountryId { get; set; }
            public long PaymentTypeId { get; set; }
            public long? AgentId { get; set; }
            public decimal Amount { get; set; }
        }

        public class ConvertResponse
        {
            public decimal SendAmount { get; set; }
            public string SendingCurrency { get; set; } = string.Empty;
            public decimal CustomerRate { get; set; }
            public decimal ReceiveAmount { get; set; }
            public string ReceivingCurrency { get; set; } = string.Empty;
            public decimal? SettlementRate { get; set; }
            public decimal? SettlementAmount { get; set; }
            public string? SettlementCurrency { get; set; }
            public decimal? CrossRate { get; set; }
        }
    }
}
