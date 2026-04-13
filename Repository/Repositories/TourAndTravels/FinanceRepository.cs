using System.Data;
using Dapper;
using Repository.Interfaces.TourAndTravels;
using static Repository.DataModels.TourAndTravels.FinanceDTO;

namespace Repository.Repositories.TourAndTravels
{
    public class FinanceRepository : IFinanceRepository
    {
        private readonly IDbConnection _db;

        public FinanceRepository(IDbConnection dbConnection)
        {
            _db = dbConnection;
        }

        private void EnsureOpen()
        {
            if (_db.State != ConnectionState.Open) _db.Open();
        }

        // ════════════════════════════════════════
        // TAX CONFIG
        // ════════════════════════════════════════

        public TaxConfigRecord CreateTaxConfig(long tenantId, string taxName, decimal taxRate, string taxType, string? panNumber, string? vatNumber)
        {
            EnsureOpen();
            const string sql = @"
                INSERT INTO tnt_tax_configs (tenant_id, tax_name, tax_rate, tax_type, pan_number, vat_number, is_active, created_at)
                VALUES (@TenantId, @TaxName, @TaxRate, @TaxType, @PanNumber, @VatNumber, true, NOW())
                RETURNING *;";
            return _db.QuerySingle<TaxConfigRecord>(sql, new { TenantId = tenantId, TaxName = taxName, TaxRate = taxRate, TaxType = taxType, PanNumber = panNumber, VatNumber = vatNumber });
        }

        public TaxConfigRecord? GetTaxConfig(long tenantId)
        {
            EnsureOpen();
            return _db.QueryFirstOrDefault<TaxConfigRecord>(
                "SELECT * FROM tnt_tax_configs WHERE tenant_id = @TenantId AND is_active = true ORDER BY id DESC LIMIT 1;",
                new { TenantId = tenantId });
        }

        public IEnumerable<TaxConfigRecord> GetAllTaxConfigs(long tenantId)
        {
            EnsureOpen();
            return _db.Query<TaxConfigRecord>(
                "SELECT * FROM tnt_tax_configs WHERE tenant_id = @TenantId ORDER BY id DESC;",
                new { TenantId = tenantId });
        }

        public bool UpdateTaxConfig(long id, string taxName, decimal taxRate, string taxType, string? panNumber, string? vatNumber)
        {
            EnsureOpen();
            const string sql = @"
                UPDATE tnt_tax_configs SET tax_name=@TaxName, tax_rate=@TaxRate, tax_type=@TaxType,
                    pan_number=@PanNumber, vat_number=@VatNumber
                WHERE id=@Id;";
            return _db.Execute(sql, new { Id = id, TaxName = taxName, TaxRate = taxRate, TaxType = taxType, PanNumber = panNumber, VatNumber = vatNumber }) > 0;
        }

        // ════════════════════════════════════════
        // INVOICE
        // ════════════════════════════════════════

        public InvoiceRecord CreateInvoice(long tenantId, long bookingId, string currency, decimal exchangeRate,
            DateTime? dueDate, decimal discountAmount, decimal subtotal, decimal taxAmount, decimal totalAmount,
            string customerName, string? customerEmail, string? customerPhone, string? customerAddress,
            string? panNumber, string? vatNumber, string? notes, string? createdBy,
            IEnumerable<(string itemType, string description, decimal quantity, decimal unitPrice, decimal discountAmount, decimal taxRate, decimal taxAmount, decimal amount, int sortOrder)> lineItems)
        {
            EnsureOpen();
            using var tx = _db.BeginTransaction();
            try
            {
                // Generate sequential invoice number
                string invoiceNumber = GenerateInvoiceNumber(tenantId);

                const string sqlInvoice = @"
                    INSERT INTO tnt_invoices
                    (tenant_id, booking_id, invoice_number, invoice_date, due_date, status, currency, exchange_rate,
                     subtotal, discount_amount, tax_amount, total_amount, amount_paid, balance_due,
                     customer_name, customer_email, customer_phone, customer_address,
                     pan_number, vat_number, notes, created_by, created_at)
                    VALUES
                    (@TenantId, @BookingId, @InvoiceNumber, NOW()::date, @DueDate, 'Draft', @Currency, @ExchangeRate,
                     @Subtotal, @DiscountAmount, @TaxAmount, @TotalAmount, 0, @TotalAmount,
                     @CustomerName, @CustomerEmail, @CustomerPhone, @CustomerAddress,
                     @PanNumber, @VatNumber, @Notes, @CreatedBy, NOW())
                    RETURNING id;";

                long invoiceId = _db.QuerySingle<long>(sqlInvoice, new
                {
                    TenantId = tenantId, BookingId = bookingId, InvoiceNumber = invoiceNumber,
                    DueDate = dueDate, Currency = currency, ExchangeRate = exchangeRate,
                    Subtotal = subtotal, DiscountAmount = discountAmount, TaxAmount = taxAmount, TotalAmount = totalAmount,
                    CustomerName = customerName, CustomerEmail = customerEmail, CustomerPhone = customerPhone, CustomerAddress = customerAddress,
                    PanNumber = panNumber, VatNumber = vatNumber, Notes = notes, CreatedBy = createdBy
                }, tx);

                foreach (var item in lineItems)
                {
                    const string sqlLine = @"
                        INSERT INTO tnt_invoice_line_items
                        (invoice_id, item_type, description, quantity, unit_price, discount_amount, tax_rate, tax_amount, amount, sort_order)
                        VALUES (@InvoiceId, @ItemType, @Description, @Quantity, @UnitPrice, @DiscountAmount, @TaxRate, @TaxAmount, @Amount, @SortOrder);";
                    _db.Execute(sqlLine, new
                    {
                        InvoiceId = invoiceId, ItemType = item.itemType, Description = item.description,
                        Quantity = item.quantity, UnitPrice = item.unitPrice, DiscountAmount = item.discountAmount,
                        TaxRate = item.taxRate, TaxAmount = item.taxAmount, Amount = item.amount, SortOrder = item.sortOrder
                    }, tx);
                }

                tx.Commit();

                return _db.QuerySingle<InvoiceRecord>(
                    @"SELECT i.*, ii.booking_reference
                      FROM tnt_invoices i
                      LEFT JOIN itinerary_instances ii ON ii.id = i.booking_id
                      WHERE i.id = @Id;",
                    new { Id = invoiceId });
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private string GenerateInvoiceNumber(long tenantId)
        {
            int count = _db.QuerySingle<int>(
                "SELECT COUNT(*) FROM tnt_invoices WHERE tenant_id = @TenantId;",
                new { TenantId = tenantId });
            return $"INV-{DateTime.UtcNow:yyyy}-{(count + 1):D5}";
        }

        public InvoiceRecord? GetInvoiceById(long id, long tenantId)
        {
            EnsureOpen();
            return _db.QueryFirstOrDefault<InvoiceRecord>(
                @"SELECT i.*, ii.booking_reference
                  FROM tnt_invoices i
                  LEFT JOIN itinerary_instances ii ON ii.id = i.booking_id
                  WHERE i.id = @Id AND i.tenant_id = @TenantId;",
                new { Id = id, TenantId = tenantId });
        }

        public IEnumerable<InvoiceRecord> GetInvoices(long tenantId, string? status = null, int page = 1, int pageSize = 20)
        {
            EnsureOpen();
            string where = status != null ? "AND i.status = @Status" : "";
            string sql = $@"
                SELECT i.*, ii.booking_reference
                FROM tnt_invoices i
                LEFT JOIN itinerary_instances ii ON ii.id = i.booking_id
                WHERE i.tenant_id = @TenantId {where}
                ORDER BY i.created_at DESC
                LIMIT @PageSize OFFSET @Offset;";
            return _db.Query<InvoiceRecord>(sql, new { TenantId = tenantId, Status = status, PageSize = pageSize, Offset = (page - 1) * pageSize });
        }

        public IEnumerable<InvoiceLineItemRecord> GetInvoiceLineItems(long invoiceId)
        {
            EnsureOpen();
            return _db.Query<InvoiceLineItemRecord>(
                "SELECT * FROM tnt_invoice_line_items WHERE invoice_id = @InvoiceId ORDER BY sort_order;",
                new { InvoiceId = invoiceId });
        }

        public bool UpdateInvoiceStatus(long id, long tenantId, string status, string? notes)
        {
            EnsureOpen();
            return _db.Execute(
                "UPDATE tnt_invoices SET status=@Status, notes=@Notes, updated_at=NOW() WHERE id=@Id AND tenant_id=@TenantId;",
                new { Id = id, TenantId = tenantId, Status = status, Notes = notes }) > 0;
        }

        public bool UpdateInvoicePayment(long invoiceId, decimal amountPaid, decimal balanceDue, string status)
        {
            EnsureOpen();
            return _db.Execute(
                "UPDATE tnt_invoices SET amount_paid=@AmountPaid, balance_due=@BalanceDue, status=@Status, updated_at=NOW() WHERE id=@InvoiceId;",
                new { InvoiceId = invoiceId, AmountPaid = amountPaid, BalanceDue = balanceDue, Status = status }) > 0;
        }

        public int GetInvoiceCount(long tenantId, string? status = null)
        {
            EnsureOpen();
            string where = status != null ? "AND status = @Status" : "";
            return _db.QuerySingle<int>($"SELECT COUNT(*) FROM tnt_invoices WHERE tenant_id = @TenantId {where};",
                new { TenantId = tenantId, Status = status });
        }

        // ════════════════════════════════════════
        // AGENT COMMISSION
        // ════════════════════════════════════════

        public CommissionRecord CreateCommission(long tenantId, long bookingId, string agentName, string? agentContact,
            string commissionType, decimal commissionRate, decimal commissionAmount, string currency, string? notes)
        {
            EnsureOpen();
            const string sql = @"
                INSERT INTO tnt_agent_commissions
                (tenant_id, booking_id, agent_name, agent_contact, commission_type, commission_rate, commission_amount, currency, status, notes, created_at)
                VALUES (@TenantId, @BookingId, @AgentName, @AgentContact, @CommissionType, @CommissionRate, @CommissionAmount, @Currency, 'Pending', @Notes, NOW())
                RETURNING id;";
            long id = _db.QuerySingle<long>(sql, new { TenantId = tenantId, BookingId = bookingId, AgentName = agentName, AgentContact = agentContact, CommissionType = commissionType, CommissionRate = commissionRate, CommissionAmount = commissionAmount, Currency = currency, Notes = notes });
            return GetCommissionById(id, tenantId)!;
        }

        public CommissionRecord? GetCommissionById(long id, long tenantId)
        {
            EnsureOpen();
            return _db.QueryFirstOrDefault<CommissionRecord>(
                @"SELECT c.*, ii.booking_reference
                  FROM tnt_agent_commissions c
                  LEFT JOIN itinerary_instances ii ON ii.id = c.booking_id
                  WHERE c.id = @Id AND c.tenant_id = @TenantId;",
                new { Id = id, TenantId = tenantId });
        }

        public IEnumerable<CommissionRecord> GetCommissions(long tenantId, string? status = null)
        {
            EnsureOpen();
            string where = status != null ? "AND c.status = @Status" : "";
            string sql = $@"
                SELECT c.*, ii.booking_reference
                FROM tnt_agent_commissions c
                LEFT JOIN itinerary_instances ii ON ii.id = c.booking_id
                WHERE c.tenant_id = @TenantId {where}
                ORDER BY c.created_at DESC;";
            return _db.Query<CommissionRecord>(sql, new { TenantId = tenantId, Status = status });
        }

        public IEnumerable<CommissionRecord> GetCommissionsByBooking(long bookingId)
        {
            EnsureOpen();
            return _db.Query<CommissionRecord>(
                @"SELECT c.*, ii.booking_reference
                  FROM tnt_agent_commissions c
                  LEFT JOIN itinerary_instances ii ON ii.id = c.booking_id
                  WHERE c.booking_id = @BookingId ORDER BY c.created_at DESC;",
                new { BookingId = bookingId });
        }

        public bool UpdateCommissionStatus(long id, long tenantId, string status, DateTime? paymentDate, string? notes)
        {
            EnsureOpen();
            return _db.Execute(
                "UPDATE tnt_agent_commissions SET status=@Status, payment_date=@PaymentDate, notes=@Notes WHERE id=@Id AND tenant_id=@TenantId;",
                new { Id = id, TenantId = tenantId, Status = status, PaymentDate = paymentDate, Notes = notes }) > 0;
        }

        // ════════════════════════════════════════
        // EXPENSES
        // ════════════════════════════════════════

        public ExpenseRecord CreateExpense(long tenantId, long? bookingId, string category, string description,
            decimal amount, string currency, DateTime expenseDate, string? paidTo, string? paymentMethod,
            string? referenceNumber, string? receiptUrl, string? notes, string? createdBy)
        {
            EnsureOpen();
            const string sql = @"
                INSERT INTO tnt_expenses
                (tenant_id, booking_id, category, description, amount, currency, expense_date, paid_to, payment_method, reference_number, receipt_url, notes, created_by, created_at)
                VALUES (@TenantId, @BookingId, @Category, @Description, @Amount, @Currency, @ExpenseDate, @PaidTo, @PaymentMethod, @ReferenceNumber, @ReceiptUrl, @Notes, @CreatedBy, NOW())
                RETURNING id;";
            long id = _db.QuerySingle<long>(sql, new { TenantId = tenantId, BookingId = bookingId, Category = category, Description = description, Amount = amount, Currency = currency, ExpenseDate = expenseDate, PaidTo = paidTo, PaymentMethod = paymentMethod, ReferenceNumber = referenceNumber, ReceiptUrl = receiptUrl, Notes = notes, CreatedBy = createdBy });
            return GetExpenseById(id, tenantId)!;
        }

        public ExpenseRecord? GetExpenseById(long id, long tenantId)
        {
            EnsureOpen();
            return _db.QueryFirstOrDefault<ExpenseRecord>(
                @"SELECT e.*, ii.booking_reference
                  FROM tnt_expenses e
                  LEFT JOIN itinerary_instances ii ON ii.id = e.booking_id
                  WHERE e.id = @Id AND e.tenant_id = @TenantId;",
                new { Id = id, TenantId = tenantId });
        }

        public IEnumerable<ExpenseRecord> GetExpenses(long tenantId, long? bookingId = null, string? category = null)
        {
            EnsureOpen();
            string where = "";
            if (bookingId.HasValue) where += " AND e.booking_id = @BookingId";
            if (category != null) where += " AND e.category = @Category";
            string sql = $@"
                SELECT e.*, ii.booking_reference
                FROM tnt_expenses e
                LEFT JOIN itinerary_instances ii ON ii.id = e.booking_id
                WHERE e.tenant_id = @TenantId {where}
                ORDER BY e.expense_date DESC;";
            return _db.Query<ExpenseRecord>(sql, new { TenantId = tenantId, BookingId = bookingId, Category = category });
        }

        public bool UpdateExpense(long id, long tenantId, string category, string description, decimal amount,
            string currency, DateTime expenseDate, string? paidTo, string? paymentMethod,
            string? referenceNumber, string? receiptUrl, string? notes)
        {
            EnsureOpen();
            return _db.Execute(@"
                UPDATE tnt_expenses SET category=@Category, description=@Description, amount=@Amount, currency=@Currency,
                    expense_date=@ExpenseDate, paid_to=@PaidTo, payment_method=@PaymentMethod,
                    reference_number=@ReferenceNumber, receipt_url=@ReceiptUrl, notes=@Notes
                WHERE id=@Id AND tenant_id=@TenantId;",
                new { Id = id, TenantId = tenantId, Category = category, Description = description, Amount = amount, Currency = currency, ExpenseDate = expenseDate, PaidTo = paidTo, PaymentMethod = paymentMethod, ReferenceNumber = referenceNumber, ReceiptUrl = receiptUrl, Notes = notes }) > 0;
        }

        public bool DeleteExpense(long id, long tenantId)
        {
            EnsureOpen();
            return _db.Execute("DELETE FROM tnt_expenses WHERE id=@Id AND tenant_id=@TenantId;", new { Id = id, TenantId = tenantId }) > 0;
        }

        // ════════════════════════════════════════
        // REFUNDS
        // ════════════════════════════════════════

        public RefundRecord CreateRefund(long tenantId, long bookingId, decimal originalAmount, decimal refundAmount,
            decimal cancellationFee, string currency, string reason, string? refundMethod, string? notes)
        {
            EnsureOpen();
            string refundNumber = $"RFN-{DateTime.UtcNow:yyyy}-{new Random().Next(10000, 99999)}";
            const string sql = @"
                INSERT INTO tnt_refunds
                (tenant_id, booking_id, refund_number, original_amount, refund_amount, cancellation_fee, currency, reason, status, refund_method, notes, created_at)
                VALUES (@TenantId, @BookingId, @RefundNumber, @OriginalAmount, @RefundAmount, @CancellationFee, @Currency, @Reason, 'Pending', @RefundMethod, @Notes, NOW())
                RETURNING id;";
            long id = _db.QuerySingle<long>(sql, new { TenantId = tenantId, BookingId = bookingId, RefundNumber = refundNumber, OriginalAmount = originalAmount, RefundAmount = refundAmount, CancellationFee = cancellationFee, Currency = currency, Reason = reason, RefundMethod = refundMethod, Notes = notes });
            return GetRefundById(id, tenantId)!;
        }

        public RefundRecord? GetRefundById(long id, long tenantId)
        {
            EnsureOpen();
            return _db.QueryFirstOrDefault<RefundRecord>(
                @"SELECT r.*, ii.booking_reference
                  FROM tnt_refunds r
                  LEFT JOIN itinerary_instances ii ON ii.id = r.booking_id
                  WHERE r.id = @Id AND r.tenant_id = @TenantId;",
                new { Id = id, TenantId = tenantId });
        }

        public IEnumerable<RefundRecord> GetRefunds(long tenantId, string? status = null)
        {
            EnsureOpen();
            string where = status != null ? "AND r.status = @Status" : "";
            string sql = $@"
                SELECT r.*, ii.booking_reference
                FROM tnt_refunds r
                LEFT JOIN itinerary_instances ii ON ii.id = r.booking_id
                WHERE r.tenant_id = @TenantId {where}
                ORDER BY r.created_at DESC;";
            return _db.Query<RefundRecord>(sql, new { TenantId = tenantId, Status = status });
        }

        public bool ProcessRefund(long id, long tenantId, string status, string? transactionReference, string? processedBy, string? notes)
        {
            EnsureOpen();
            return _db.Execute(@"
                UPDATE tnt_refunds SET status=@Status, transaction_reference=@TransactionReference,
                    processed_by=@ProcessedBy, processed_at=CASE WHEN @Status='Processed' THEN NOW() ELSE NULL END, notes=@Notes
                WHERE id=@Id AND tenant_id=@TenantId;",
                new { Id = id, TenantId = tenantId, Status = status, TransactionReference = transactionReference, ProcessedBy = processedBy, Notes = notes }) > 0;
        }

        // ════════════════════════════════════════
        // SUMMARY
        // ════════════════════════════════════════

        public FinanceSummaryRecord GetFinanceSummary(long tenantId)
        {
            EnsureOpen();
            const string sql = @"
                SELECT
                    COALESCE((SELECT SUM(total_amount) FROM tnt_invoices WHERE tenant_id = @TenantId AND status != 'Cancelled'), 0) AS TotalRevenue,
                    COALESCE((SELECT SUM(amount_paid) FROM tnt_invoices WHERE tenant_id = @TenantId), 0) AS TotalReceived,
                    COALESCE((SELECT SUM(balance_due) FROM tnt_invoices WHERE tenant_id = @TenantId AND status NOT IN ('Paid','Cancelled')), 0) AS TotalOutstanding,
                    COALESCE((SELECT SUM(amount) FROM tnt_expenses WHERE tenant_id = @TenantId), 0) AS TotalExpenses,
                    COALESCE((SELECT SUM(commission_amount) FROM tnt_agent_commissions WHERE tenant_id = @TenantId AND status != 'Cancelled'), 0) AS TotalCommissions,
                    COALESCE((SELECT SUM(refund_amount) FROM tnt_refunds WHERE tenant_id = @TenantId AND status = 'Processed'), 0) AS TotalRefunds,
                    (SELECT COUNT(*) FROM tnt_invoices WHERE tenant_id = @TenantId) AS TotalInvoices,
                    (SELECT COUNT(*) FROM tnt_invoices WHERE tenant_id = @TenantId AND status = 'Paid') AS PaidInvoices,
                    (SELECT COUNT(*) FROM tnt_invoices WHERE tenant_id = @TenantId AND status = 'Overdue') AS OverdueInvoices,
                    (SELECT COUNT(*) FROM tnt_refunds WHERE tenant_id = @TenantId AND status = 'Pending') AS PendingRefunds;";
            var record = _db.QuerySingle<FinanceSummaryRecord>(sql, new { TenantId = tenantId });
            record.NetProfit = record.TotalReceived - record.TotalExpenses - record.TotalCommissions - record.TotalRefunds;
            return record;
        }
    }
}
