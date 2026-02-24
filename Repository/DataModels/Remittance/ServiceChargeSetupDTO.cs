using System;
using System.Collections.Generic;

namespace Repository.DataModels.Remittance
{
    public class ServiceChargeSetupDTO
    {
        public class CreateSetupRequest
        {
            public long SendingCountryId { get; set; }
            public long ReceivingCountryId { get; set; }
            public long PaymentTypeId { get; set; }
            public long? AgentId { get; set; }
            public string ChargeMode { get; set; } = "Range";
            public List<SlabRequest> Slabs { get; set; } = new();
        }

        public class SlabRequest
        {
            public decimal MinAmount { get; set; }
            public decimal MaxAmount { get; set; }
            public string ChargeType { get; set; } = "Flat";
            public decimal ChargeValue { get; set; }
            public string Currency { get; set; } = "USD";
        }

        public class UpdateSetupRequest
        {
            public string? ChargeMode { get; set; }
            public bool? IsActive { get; set; }
            public List<SlabRequest>? Slabs { get; set; }
        }

        public class SetupListItem
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
            public string ChargeMode { get; set; } = "Range";
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

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
            public string Currency { get; set; } = "USD";
        }

        public class CalculateChargeRequest
        {
            public long SendingCountryId { get; set; }
            public long ReceivingCountryId { get; set; }
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
            public string Currency { get; set; } = "USD";
        }
    }
}
