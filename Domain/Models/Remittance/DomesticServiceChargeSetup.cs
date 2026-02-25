using System;
using System.Collections.Generic;

namespace Domain.Models.Remittance
{
    public class DomesticServiceChargeSetup
    {
        // ===========================
        // Create
        // ===========================
        public class CreateSetupRequest
        {
            public long FromCategoryId { get; set; }
            public long ToCategoryId { get; set; }
            public long PaymentTypeId { get; set; }
            public long? AgentId { get; set; }
            public string ChargeMode { get; set; } = "Range"; // 'Flat' or 'Range'
            public List<SlabRequest> Slabs { get; set; } = new();
        }

        public class SlabRequest
        {
            public decimal MinAmount { get; set; }
            public decimal MaxAmount { get; set; }
            public string ChargeType { get; set; } = "Flat"; // 'Flat' or 'Percentage'
            public decimal ChargeValue { get; set; }
            public string Currency { get; set; } = "NPR";
        }

        // ===========================
        // Update
        // ===========================
        public class UpdateSetupRequest
        {
            public string? ChargeMode { get; set; }
            public bool? IsActive { get; set; }
            public List<SlabRequest>? Slabs { get; set; }
        }

        // ===========================
        // Response (list item)
        // ===========================
        public class SetupListItem
        {
            public long Id { get; set; }
            public long FromCategoryId { get; set; }
            public string FromCategoryName { get; set; } = string.Empty;
            public long ToCategoryId { get; set; }
            public string ToCategoryName { get; set; } = string.Empty;
            public long PaymentTypeId { get; set; }
            public string PaymentTypeName { get; set; } = string.Empty;
            public long? AgentId { get; set; }
            public string? AgentName { get; set; }
            public string ChargeMode { get; set; } = "Range";
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        // ===========================
        // Response (detail with slabs)
        // ===========================
        public class SetupDetailResponse : SetupListItem
        {
            public List<SlabResponse> Slabs { get; set; } = new();
        }

        public class SlabResponse
        {
            public long Id { get; set; }
            public decimal MinAmount { get; set; }
            public decimal MaxAmount { get; set; }
            public string ChargeType { get; set; } = "Flat";
            public decimal ChargeValue { get; set; }
            public string Currency { get; set; } = "NPR";
        }

        // ===========================
        // Calculate charge (utility request/response)
        // ===========================
        public class CalculateChargeRequest
        {
            public long FromCategoryId { get; set; }
            public long ToCategoryId { get; set; }
            public long PaymentTypeId { get; set; }
            public long? AgentId { get; set; }
            public decimal Amount { get; set; }
        }

        public class CalculateChargeResponse
        {
            public decimal SendAmount { get; set; }
            public decimal ServiceCharge { get; set; }
            public string ChargeType { get; set; } = "Flat";
            public decimal ChargeValue { get; set; }
            public decimal TotalDeducted { get; set; }
            public string Currency { get; set; } = "NPR";
        }
    }
}
