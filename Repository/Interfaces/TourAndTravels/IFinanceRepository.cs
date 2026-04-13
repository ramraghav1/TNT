using static Repository.DataModels.TourAndTravels.FinanceDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IFinanceRepository
    {
        // ─── Tax Config ───────────────────────────────────────────────
        TaxConfigRecord CreateTaxConfig(long tenantId, string taxName, decimal taxRate, string taxType, string? panNumber, string? vatNumber);
        TaxConfigRecord? GetTaxConfig(long tenantId);
        IEnumerable<TaxConfigRecord> GetAllTaxConfigs(long tenantId);
        bool UpdateTaxConfig(long id, string taxName, decimal taxRate, string taxType, string? panNumber, string? vatNumber);

        // ─── Invoice ──────────────────────────────────────────────────
        InvoiceRecord CreateInvoice(long tenantId, long bookingId, string currency, decimal exchangeRate,
            DateTime? dueDate, decimal discountAmount, decimal subtotal, decimal taxAmount, decimal totalAmount,
            string customerName, string? customerEmail, string? customerPhone, string? customerAddress,
            string? panNumber, string? vatNumber, string? notes, string? createdBy,
            IEnumerable<(string itemType, string description, decimal quantity, decimal unitPrice, decimal discountAmount, decimal taxRate, decimal taxAmount, decimal amount, int sortOrder)> lineItems);
        InvoiceRecord? GetInvoiceById(long id, long tenantId);
        IEnumerable<InvoiceRecord> GetInvoices(long tenantId, string? status = null, int page = 1, int pageSize = 20);
        IEnumerable<InvoiceLineItemRecord> GetInvoiceLineItems(long invoiceId);
        bool UpdateInvoiceStatus(long id, long tenantId, string status, string? notes);
        bool UpdateInvoicePayment(long invoiceId, decimal amountPaid, decimal balanceDue, string status);
        int GetInvoiceCount(long tenantId, string? status = null);

        // ─── Commission ───────────────────────────────────────────────
        CommissionRecord CreateCommission(long tenantId, long bookingId, string agentName, string? agentContact,
            string commissionType, decimal commissionRate, decimal commissionAmount, string currency, string? notes);
        CommissionRecord? GetCommissionById(long id, long tenantId);
        IEnumerable<CommissionRecord> GetCommissions(long tenantId, string? status = null);
        IEnumerable<CommissionRecord> GetCommissionsByBooking(long bookingId);
        bool UpdateCommissionStatus(long id, long tenantId, string status, DateTime? paymentDate, string? notes);

        // ─── Expense ──────────────────────────────────────────────────
        ExpenseRecord CreateExpense(long tenantId, long? bookingId, string category, string description,
            decimal amount, string currency, DateTime expenseDate, string? paidTo, string? paymentMethod,
            string? referenceNumber, string? receiptUrl, string? notes, string? createdBy);
        ExpenseRecord? GetExpenseById(long id, long tenantId);
        IEnumerable<ExpenseRecord> GetExpenses(long tenantId, long? bookingId = null, string? category = null);
        bool UpdateExpense(long id, long tenantId, string category, string description, decimal amount,
            string currency, DateTime expenseDate, string? paidTo, string? paymentMethod,
            string? referenceNumber, string? receiptUrl, string? notes);
        bool DeleteExpense(long id, long tenantId);

        // ─── Refund ───────────────────────────────────────────────────
        RefundRecord CreateRefund(long tenantId, long bookingId, decimal originalAmount, decimal refundAmount,
            decimal cancellationFee, string currency, string reason, string? refundMethod, string? notes);
        RefundRecord? GetRefundById(long id, long tenantId);
        IEnumerable<RefundRecord> GetRefunds(long tenantId, string? status = null);
        bool ProcessRefund(long id, long tenantId, string status, string? transactionReference, string? processedBy, string? notes);

        // ─── Summary ──────────────────────────────────────────────────
        FinanceSummaryRecord GetFinanceSummary(long tenantId);
    }
}
