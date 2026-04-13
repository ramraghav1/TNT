using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;
using Repository.Repositories;
using static Domain.Models.TourAndTravels.Finance;

namespace Bussiness.Services.TourAndTravels
{
    public interface IFinanceService
    {
        // Tax Config
        TaxConfigResponse CreateTaxConfig(CreateTaxConfigRequest request);
        TaxConfigResponse? GetActiveTaxConfig();
        IEnumerable<TaxConfigResponse> GetAllTaxConfigs();
        bool UpdateTaxConfig(long id, CreateTaxConfigRequest request);

        // Invoices
        InvoiceResponse GenerateInvoiceFromBooking(long bookingId, string? notes);
        InvoiceResponse CreateInvoice(CreateInvoiceRequest request);
        InvoiceResponse? GetInvoiceById(long id);
        (IEnumerable<InvoiceListItem> Items, int Total) GetInvoices(string? status, int page, int pageSize);
        bool UpdateInvoiceStatus(long id, UpdateInvoiceStatusRequest request);

        // Commissions
        CommissionResponse CreateCommission(CreateCommissionRequest request);
        CommissionResponse? GetCommissionById(long id);
        IEnumerable<CommissionResponse> GetCommissions(string? status);
        IEnumerable<CommissionResponse> GetCommissionsByBooking(long bookingId);
        bool UpdateCommissionStatus(long id, UpdateCommissionStatusRequest request);

        // Expenses
        ExpenseResponse CreateExpense(CreateExpenseRequest request);
        ExpenseResponse? GetExpenseById(long id);
        IEnumerable<ExpenseResponse> GetExpenses(long? bookingId, string? category);
        bool UpdateExpense(long id, UpdateExpenseRequest request);
        bool DeleteExpense(long id);

        // Refunds
        RefundResponse CreateRefund(CreateRefundRequest request);
        RefundResponse? GetRefundById(long id);
        IEnumerable<RefundResponse> GetRefunds(string? status);
        bool ProcessRefund(long id, ProcessRefundRequest request, string processedBy);

        // Summary
        FinanceSummary GetFinanceSummary();
    }

    public class FinanceService : IFinanceService
    {
        private readonly IFinanceRepository _repo;
        private readonly ITenantProvider _tenantProvider;
        // Inject IBookingRepository to fetch booking details for invoice auto-generation
        private readonly IBookingRepository _bookingRepo;

        public FinanceService(IFinanceRepository repo, ITenantProvider tenantProvider, IBookingRepository bookingRepo)
        {
            _repo = repo;
            _tenantProvider = tenantProvider;
            _bookingRepo = bookingRepo;
        }

        private long TenantId => _tenantProvider.GetRequiredTenantId();

        // ─── Tax Config ───────────────────────────────────────────────

        public TaxConfigResponse CreateTaxConfig(CreateTaxConfigRequest request)
        {
            var rec = _repo.CreateTaxConfig(TenantId, request.TaxName, request.TaxRate, request.TaxType, request.PanNumber, request.VatNumber);
            return MapTaxConfig(rec);
        }

        public TaxConfigResponse? GetActiveTaxConfig()
        {
            var rec = _repo.GetTaxConfig(TenantId);
            return rec == null ? null : MapTaxConfig(rec);
        }

        public IEnumerable<TaxConfigResponse> GetAllTaxConfigs()
        {
            return _repo.GetAllTaxConfigs(TenantId).Select(MapTaxConfig);
        }

        public bool UpdateTaxConfig(long id, CreateTaxConfigRequest request)
        {
            return _repo.UpdateTaxConfig(id, request.TaxName, request.TaxRate, request.TaxType, request.PanNumber, request.VatNumber);
        }

        // ─── Invoices ─────────────────────────────────────────────────

        public InvoiceResponse GenerateInvoiceFromBooking(long bookingId, string? notes)
        {
            // Load booking details to auto-populate invoice
            var booking = _bookingRepo.GetBookingById(bookingId)
                ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

            var taxConfig = _repo.GetTaxConfig(TenantId);
            decimal taxRate = taxConfig?.TaxRate ?? 0;

            // Build line items from booking
            var lineItems = new List<(string itemType, string description, decimal quantity, decimal unitPrice, decimal discountAmount, decimal taxRate, decimal taxAmount, decimal amount, int sortOrder)>();

            // Main package line item
            decimal packageAmount = booking.TotalAmount;
            decimal packageTax = Math.Round(packageAmount * taxRate / 100, 2);
            lineItems.Add(("Package", $"Tour Package - {booking.TemplateTitle}", 1, packageAmount, 0, taxRate, packageTax, packageAmount + packageTax, 1));

            decimal subtotal = packageAmount;
            decimal itemTaxAmount = packageTax;
            decimal totalAmount = subtotal + itemTaxAmount;

            // Get primary traveler name
            var primaryTraveler = booking.Travelers.FirstOrDefault();
            string customerName = primaryTraveler?.FullName ?? "Guest";
            string? customerEmail = primaryTraveler?.Email;
            string? customerPhone = primaryTraveler?.ContactNumber;

            var rec = _repo.CreateInvoice(
                TenantId, bookingId, "NPR", 1,
                DateTime.UtcNow.AddDays(30), 0,
                subtotal, itemTaxAmount, totalAmount,
                customerName, customerEmail, customerPhone, null,
                taxConfig?.PanNumber, taxConfig?.VatNumber,
                notes, null, lineItems);

            return BuildInvoiceResponse(rec);
        }

        public InvoiceResponse CreateInvoice(CreateInvoiceRequest request)
        {
            var taxConfig = _repo.GetTaxConfig(TenantId);
            decimal taxRate = taxConfig?.TaxRate ?? 0;

            var lineItems = request.LineItems.Select(li =>
            {
                decimal effectiveTaxRate = request.ApplyTax ? (li.TaxRate > 0 ? li.TaxRate : taxRate) : 0;
                decimal lineAmount = li.Quantity * li.UnitPrice - li.DiscountAmount;
                decimal lineTax = Math.Round(lineAmount * effectiveTaxRate / 100, 2);
                return (li.ItemType, li.Description, li.Quantity, li.UnitPrice, li.DiscountAmount, effectiveTaxRate, lineTax, lineAmount + lineTax, li.SortOrder);
            }).ToList();

            decimal subtotal = lineItems.Sum(li => li.Quantity * li.UnitPrice - li.DiscountAmount);
            decimal taxAmount = lineItems.Sum(li => li.lineTax);
            decimal totalAmount = subtotal - request.DiscountAmount + taxAmount;

            // Fetch customer from booking travelers
            var bookingDetail = _bookingRepo.GetBookingById(request.BookingId);
            var primaryTraveler = bookingDetail?.Travelers.FirstOrDefault();

            var rec = _repo.CreateInvoice(
                TenantId, request.BookingId, request.Currency, request.ExchangeRate,
                request.DueDate, request.DiscountAmount,
                subtotal, taxAmount, totalAmount,
                primaryTraveler?.FullName ?? "Guest",
                primaryTraveler?.Email, primaryTraveler?.ContactNumber, null,
                taxConfig?.PanNumber, taxConfig?.VatNumber,
                request.Notes, null, lineItems);

            return BuildInvoiceResponse(rec);
        }

        public InvoiceResponse? GetInvoiceById(long id)
        {
            var rec = _repo.GetInvoiceById(id, TenantId);
            if (rec == null) return null;
            return BuildInvoiceResponse(rec);
        }

        public (IEnumerable<InvoiceListItem> Items, int Total) GetInvoices(string? status, int page, int pageSize)
        {
            var items = _repo.GetInvoices(TenantId, status, page, pageSize).Select(MapInvoiceListItem);
            int total = _repo.GetInvoiceCount(TenantId, status);
            return (items, total);
        }

        public bool UpdateInvoiceStatus(long id, UpdateInvoiceStatusRequest request)
        {
            return _repo.UpdateInvoiceStatus(id, TenantId, request.Status, request.Notes);
        }

        // ─── Commissions ──────────────────────────────────────────────

        public CommissionResponse CreateCommission(CreateCommissionRequest request)
        {
            var rec = _repo.CreateCommission(TenantId, request.BookingId, request.AgentName, request.AgentContact,
                request.CommissionType, request.CommissionRate, request.CommissionAmount, request.Currency, request.Notes);
            return MapCommission(rec);
        }

        public CommissionResponse? GetCommissionById(long id)
        {
            var rec = _repo.GetCommissionById(id, TenantId);
            return rec == null ? null : MapCommission(rec);
        }

        public IEnumerable<CommissionResponse> GetCommissions(string? status)
        {
            return _repo.GetCommissions(TenantId, status).Select(MapCommission);
        }

        public IEnumerable<CommissionResponse> GetCommissionsByBooking(long bookingId)
        {
            return _repo.GetCommissionsByBooking(bookingId).Select(MapCommission);
        }

        public bool UpdateCommissionStatus(long id, UpdateCommissionStatusRequest request)
        {
            return _repo.UpdateCommissionStatus(id, TenantId, request.Status, request.PaymentDate, request.Notes);
        }

        // ─── Expenses ─────────────────────────────────────────────────

        public ExpenseResponse CreateExpense(CreateExpenseRequest request)
        {
            var rec = _repo.CreateExpense(TenantId, request.BookingId, request.Category, request.Description,
                request.Amount, request.Currency, request.ExpenseDate, request.PaidTo, request.PaymentMethod,
                request.ReferenceNumber, request.ReceiptUrl, request.Notes, null);
            return MapExpense(rec);
        }

        public ExpenseResponse? GetExpenseById(long id)
        {
            var rec = _repo.GetExpenseById(id, TenantId);
            return rec == null ? null : MapExpense(rec);
        }

        public IEnumerable<ExpenseResponse> GetExpenses(long? bookingId, string? category)
        {
            return _repo.GetExpenses(TenantId, bookingId, category).Select(MapExpense);
        }

        public bool UpdateExpense(long id, UpdateExpenseRequest request)
        {
            return _repo.UpdateExpense(id, TenantId, request.Category, request.Description, request.Amount,
                request.Currency, request.ExpenseDate, request.PaidTo, request.PaymentMethod,
                request.ReferenceNumber, request.ReceiptUrl, request.Notes);
        }

        public bool DeleteExpense(long id)
        {
            return _repo.DeleteExpense(id, TenantId);
        }

        // ─── Refunds ──────────────────────────────────────────────────

        public RefundResponse CreateRefund(CreateRefundRequest request)
        {
            var rec = _repo.CreateRefund(TenantId, request.BookingId, request.OriginalAmount, request.RefundAmount,
                request.CancellationFee, request.Currency, request.Reason, request.RefundMethod, request.Notes);
            return MapRefund(rec);
        }

        public RefundResponse? GetRefundById(long id)
        {
            var rec = _repo.GetRefundById(id, TenantId);
            return rec == null ? null : MapRefund(rec);
        }

        public IEnumerable<RefundResponse> GetRefunds(string? status)
        {
            return _repo.GetRefunds(TenantId, status).Select(MapRefund);
        }

        public bool ProcessRefund(long id, ProcessRefundRequest request, string processedBy)
        {
            return _repo.ProcessRefund(id, TenantId, request.Status, request.TransactionReference, processedBy, request.Notes);
        }

        // ─── Summary ──────────────────────────────────────────────────

        public FinanceSummary GetFinanceSummary()
        {
            var rec = _repo.GetFinanceSummary(TenantId);
            return new FinanceSummary
            {
                TotalRevenue = rec.TotalRevenue,
                TotalReceived = rec.TotalReceived,
                TotalOutstanding = rec.TotalOutstanding,
                TotalExpenses = rec.TotalExpenses,
                TotalCommissions = rec.TotalCommissions,
                TotalRefunds = rec.TotalRefunds,
                NetProfit = rec.NetProfit,
                TotalInvoices = rec.TotalInvoices,
                PaidInvoices = rec.PaidInvoices,
                OverdueInvoices = rec.OverdueInvoices,
                PendingRefunds = rec.PendingRefunds
            };
        }

        // ─── Mapping helpers ──────────────────────────────────────────

        private InvoiceResponse BuildInvoiceResponse(Repository.DataModels.TourAndTravels.FinanceDTO.InvoiceRecord rec)
        {
            var lineItems = _repo.GetInvoiceLineItems(rec.Id).Select(li => new InvoiceLineItemResponse
            {
                Id = li.Id, ItemType = li.ItemType, Description = li.Description, Quantity = li.Quantity,
                UnitPrice = li.UnitPrice, DiscountAmount = li.DiscountAmount, TaxRate = li.TaxRate,
                TaxAmount = li.TaxAmount, Amount = li.Amount, SortOrder = li.SortOrder
            }).ToList();

            return new InvoiceResponse
            {
                Id = rec.Id, TenantId = rec.TenantId, BookingId = rec.BookingId,
                BookingReference = rec.BookingReference, InvoiceNumber = rec.InvoiceNumber,
                InvoiceDate = rec.InvoiceDate, DueDate = rec.DueDate, Status = rec.Status,
                Currency = rec.Currency, ExchangeRate = rec.ExchangeRate,
                Subtotal = rec.Subtotal, DiscountAmount = rec.DiscountAmount, TaxAmount = rec.TaxAmount,
                TotalAmount = rec.TotalAmount, AmountPaid = rec.AmountPaid, BalanceDue = rec.BalanceDue,
                CustomerName = rec.CustomerName, CustomerEmail = rec.CustomerEmail,
                CustomerPhone = rec.CustomerPhone, CustomerAddress = rec.CustomerAddress,
                PanNumber = rec.PanNumber, VatNumber = rec.VatNumber, Notes = rec.Notes,
                CreatedBy = rec.CreatedBy, CreatedAt = rec.CreatedAt, UpdatedAt = rec.UpdatedAt,
                LineItems = lineItems
            };
        }

        private static InvoiceListItem MapInvoiceListItem(Repository.DataModels.TourAndTravels.FinanceDTO.InvoiceRecord r) =>
            new() { Id = r.Id, InvoiceNumber = r.InvoiceNumber, InvoiceDate = r.InvoiceDate, DueDate = r.DueDate, Status = r.Status, CustomerName = r.CustomerName, BookingReference = r.BookingReference, TotalAmount = r.TotalAmount, AmountPaid = r.AmountPaid, BalanceDue = r.BalanceDue, Currency = r.Currency, CreatedAt = r.CreatedAt };

        private static TaxConfigResponse MapTaxConfig(Repository.DataModels.TourAndTravels.FinanceDTO.TaxConfigRecord r) =>
            new() { Id = r.Id, TenantId = r.TenantId, TaxName = r.TaxName, TaxRate = r.TaxRate, TaxType = r.TaxType, PanNumber = r.PanNumber, VatNumber = r.VatNumber, IsActive = r.IsActive, CreatedAt = r.CreatedAt };

        private static CommissionResponse MapCommission(Repository.DataModels.TourAndTravels.FinanceDTO.CommissionRecord r) =>
            new() { Id = r.Id, BookingId = r.BookingId, BookingReference = r.BookingReference, AgentName = r.AgentName, AgentContact = r.AgentContact, CommissionType = r.CommissionType, CommissionRate = r.CommissionRate, CommissionAmount = r.CommissionAmount, Currency = r.Currency, Status = r.Status, PaymentDate = r.PaymentDate, Notes = r.Notes, CreatedAt = r.CreatedAt };

        private static ExpenseResponse MapExpense(Repository.DataModels.TourAndTravels.FinanceDTO.ExpenseRecord r) =>
            new() { Id = r.Id, TenantId = r.TenantId, BookingId = r.BookingId, BookingReference = r.BookingReference, Category = r.Category, Description = r.Description, Amount = r.Amount, Currency = r.Currency, ExpenseDate = r.ExpenseDate, PaidTo = r.PaidTo, PaymentMethod = r.PaymentMethod, ReferenceNumber = r.ReferenceNumber, ReceiptUrl = r.ReceiptUrl, Notes = r.Notes, CreatedBy = r.CreatedBy, CreatedAt = r.CreatedAt };

        private static RefundResponse MapRefund(Repository.DataModels.TourAndTravels.FinanceDTO.RefundRecord r) =>
            new() { Id = r.Id, BookingId = r.BookingId, BookingReference = r.BookingReference, RefundNumber = r.RefundNumber, OriginalAmount = r.OriginalAmount, RefundAmount = r.RefundAmount, CancellationFee = r.CancellationFee, Currency = r.Currency, Reason = r.Reason, Status = r.Status, RefundMethod = r.RefundMethod, TransactionReference = r.TransactionReference, ProcessedAt = r.ProcessedAt, ProcessedBy = r.ProcessedBy, Notes = r.Notes, CreatedAt = r.CreatedAt };
    }
}
