using System;

namespace Domain.Models.Remittance
{
    public class PaymentType
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreatePaymentTypeRequest
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
        }

        public class UpdatePaymentTypeRequest
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class PaymentTypeResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
