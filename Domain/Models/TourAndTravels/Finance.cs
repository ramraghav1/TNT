namespace Domain.Models.TourAndTravels
{
    public class Finance
    {
        // ======================================
        // TAX CONFIG
        // ======================================
        public class CreateTaxConfigRequest
        {
            public string TaxName { get; set; } = string.Empty;
            public decimal TaxRate { get; set; }
            public string TaxType { get; set; } = "Percentage";
            public string? PanNumber { get; set; }
            public string? VatNumber { get; set; }
        }

        public class TaxConfigResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public string TaxName { get; set; } = string.Empty;
            public decimal TaxRate { get; set; }
            public string TaxType { get; set; } = "Percentage";
            public string? PanNumber { get; set; }
            public string? VatNumber { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ======================================
        // INVOICE
        // ======================================
        public class CreateInvoiceRequest
        {
            public long BookingId { get; set; }
            public string Currency { get; set; } = "NPR";
            public decimal ExchangeRate { get; set; } = 1;
            public DateTime? DueDate { get; set; }
            public decimal DiscountAmount { get; set; } = 0;
            public bool ApplyTax { get; set; } = true;
            public string? Notes { get; set; }
            public List<CreateInvoiceLineItemRequest> LineItems { get; set; } = new();
        }

        public class CreateInvoiceLineItemRequest
        {
            public string ItemType { get; set; } = "Package";
            public string Description { get; set; } = string.Empty;
            public decimal Quantity { get; set; } = 1;
            public decimal UnitPrice { get; set; }
            public decimal DiscountAmount { get; set; } = 0;
            public decimal TaxRate { get; set; } = 0;
            public int SortOrder { get; set; } = 0;
        }

        public class UpdateInvoiceStatusRequest
        {
            public string Status { get; set; } = string.Empty;
            public string? Notes { get; set; }
        }

        public class InvoiceResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public long BookingId { get; set; }
            public string BookingReference { get; set; } = string.Empty;
            public string InvoiceNumber { get; set; } = string.Empty;
            public DateTime InvoiceDate { get; set; }
            public DateTime? DueDate { get; set; }
            public string Status { get; set; } = "Draft";
            public string Currency { get; set; } = "NPR";
            public decimal ExchangeRate { get; set; } = 1;
            public decimal Subtotal { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal BalanceDue { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string? CustomerEmail { get; set; }
            public string? CustomerPhone { get; set; }
            public string? CustomerAddress { get; set; }
            public string? PanNumber { get; set; }
            public string? VatNumber { get; set; }
            public string? Notes { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public List<InvoiceLineItemResponse> LineItems { get; set; } = new();
        }

        public class InvoiceLineItemResponse
        {
            public long Id { get; set; }
            public string ItemType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal TaxRate { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal Amount { get; set; }
            public int SortOrder { get; set; }
        }

        public class InvoiceListItem
        {
            public long Id { get; set; }
            public string InvoiceNumber { get; set; } = string.Empty;
            public DateTime InvoiceDate { get; set; }
            public DateTime? DueDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string BookingReference { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal BalanceDue { get; set; }
            public string Currency { get; set; } = "NPR";
            public DateTime CreatedAt { get; set; }
        }

        // ======================================
        // AGENT COMMISSION
        // ======================================
        public class CreateCommissionRequest
        {
            public long BookingId { get; set; }
            public string AgentName { get; set; } = string.Empty;
            public string? AgentContact { get; set; }
            public string CommissionType { get; set; } = "Percentage";
            public decimal CommissionRate { get; set; } = 0;
            public decimal CommissionAmount { get; set; } = 0;
            public string Currency { get; set; } = "NPR";
            public string? Notes { get; set; }
        }

        public class UpdateCommissionStatusRequest
        {
            public string Status { get; set; } = string.Empty;
            public DateTime? PaymentDate { get; set; }
            public string? Notes { get; set; }
        }

        public class CommissionResponse
        {
            public long Id { get; set; }
            public long BookingId { get; set; }
            public string BookingReference { get; set; } = string.Empty;
            public string AgentName { get; set; } = string.Empty;
            public string? AgentContact { get; set; }
            public string CommissionType { get; set; } = string.Empty;
            public decimal CommissionRate { get; set; }
            public decimal CommissionAmount { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime? PaymentDate { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ======================================
        // EXPENSES
        // ======================================
        public class CreateExpenseRequest
        {
            public long? BookingId { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "NPR";
            public DateTime ExpenseDate { get; set; }
            public string? PaidTo { get; set; }
            public string? PaymentMethod { get; set; }
            public string? ReferenceNumber { get; set; }
            public string? ReceiptUrl { get; set; }
            public string? Notes { get; set; }
        }

        public class UpdateExpenseRequest : CreateExpenseRequest { }

        public class ExpenseResponse
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public long? BookingId { get; set; }
            public string? BookingReference { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Currency { get; set; } = string.Empty;
            public DateTime ExpenseDate { get; set; }
            public string? PaidTo { get; set; }
            public string? PaymentMethod { get; set; }
            public string? ReferenceNumber { get; set; }
            public string? ReceiptUrl { get; set; }
            public string? Notes { get; set; }
            public string? CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ======================================
        // REFUNDS
        // ======================================
        public class CreateRefundRequest
        {
            public long BookingId { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal RefundAmount { get; set; }
            public decimal CancellationFee { get; set; } = 0;
            public string Currency { get; set; } = "NPR";
            public string Reason { get; set; } = string.Empty;
            public string? RefundMethod { get; set; }
            public string? Notes { get; set; }
        }

        public class ProcessRefundRequest
        {
            public string Status { get; set; } = string.Empty;
            public string? TransactionReference { get; set; }
            public string? Notes { get; set; }
        }

        public class RefundResponse
        {
            public long Id { get; set; }
            public long BookingId { get; set; }
            public string BookingReference { get; set; } = string.Empty;
            public string RefundNumber { get; set; } = string.Empty;
            public decimal OriginalAmount { get; set; }
            public decimal RefundAmount { get; set; }
            public decimal CancellationFee { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string Reason { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string? RefundMethod { get; set; }
            public string? TransactionReference { get; set; }
            public DateTime? ProcessedAt { get; set; }
            public string? ProcessedBy { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ======================================
        // FINANCE SUMMARY DASHBOARD
        // ======================================
        public class FinanceSummary
        {
            public decimal TotalRevenue { get; set; }
            public decimal TotalReceived { get; set; }
            public decimal TotalOutstanding { get; set; }
            public decimal TotalExpenses { get; set; }
            public decimal TotalCommissions { get; set; }
            public decimal TotalRefunds { get; set; }
            public decimal NetProfit { get; set; }
            public int TotalInvoices { get; set; }
            public int PaidInvoices { get; set; }
            public int OverdueInvoices { get; set; }
            public int PendingRefunds { get; set; }
        }
    }
}
