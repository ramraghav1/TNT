using System;

namespace Domain.Models.Clinic
{
    public class Invoice
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreateInvoiceRequest
        {
            public long TenantId { get; set; }
            public string InvoiceNumber { get; set; } = string.Empty;
            public long PatientId { get; set; }
            public long? AppointmentId { get; set; }
            public decimal Amount { get; set; }
            public decimal Tax { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
            public string Currency { get; set; } = "NPR";
            public string Status { get; set; } = "Pending";
            public string? PaymentMethod { get; set; }
            public string? Notes { get; set; }
        }

        public class UpdateInvoiceRequest
        {
            public decimal? Amount { get; set; }
            public decimal? Tax { get; set; }
            public decimal? Discount { get; set; }
            public decimal? Total { get; set; }
            public string? Status { get; set; }
            public string? PaymentMethod { get; set; }
            public DateTime? PaidAt { get; set; }
            public string? Notes { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class InvoiceResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public string InvoiceNumber { get; set; } = string.Empty;
            public long PatientId { get; set; }
            public string? PatientName { get; set; }
            public long? AppointmentId { get; set; }
            public decimal Amount { get; set; }
            public decimal Tax { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
            public string Currency { get; set; } = "NPR";
            public string Status { get; set; } = "Pending";
            public string? PaymentMethod { get; set; }
            public DateTime? PaidAt { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
