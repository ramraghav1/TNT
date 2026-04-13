using Bussiness.Services.TourAndTravels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.Models.TourAndTravels.Finance;

namespace server.Controller.TourAndTravels
{
    [Authorize]
    [ApiController]
    [Route("api/finance")]
    public class FinanceController : ControllerBase
    {
        private readonly IFinanceService _service;

        public FinanceController(IFinanceService service)
        {
            _service = service;
        }

        // ─────────────────────────────────────────────
        // SUMMARY DASHBOARD
        // GET /api/finance/summary
        // ─────────────────────────────────────────────
        [HttpGet("summary")]
        public ActionResult<FinanceSummary> GetSummary() => Ok(_service.GetFinanceSummary());

        // ─────────────────────────────────────────────
        // TAX CONFIG
        // ─────────────────────────────────────────────
        [HttpGet("tax-config")]
        public ActionResult<IEnumerable<TaxConfigResponse>> GetTaxConfigs() => Ok(_service.GetAllTaxConfigs());

        [HttpGet("tax-config/active")]
        public ActionResult<TaxConfigResponse?> GetActiveTaxConfig() => Ok(_service.GetActiveTaxConfig());

        [HttpPost("tax-config")]
        public ActionResult<TaxConfigResponse> CreateTaxConfig([FromBody] CreateTaxConfigRequest request)
            => Ok(_service.CreateTaxConfig(request));

        [HttpPut("tax-config/{id:long}")]
        public ActionResult UpdateTaxConfig(long id, [FromBody] CreateTaxConfigRequest request)
            => _service.UpdateTaxConfig(id, request) ? NoContent() : NotFound();

        // ─────────────────────────────────────────────
        // INVOICES
        // ─────────────────────────────────────────────

        /// <summary>Auto-generates invoice from an existing booking</summary>
        [HttpPost("invoices/generate/{bookingId:long}")]
        public ActionResult<InvoiceResponse> GenerateFromBooking(long bookingId, [FromQuery] string? notes)
            => Ok(_service.GenerateInvoiceFromBooking(bookingId, notes));

        [HttpPost("invoices")]
        public ActionResult<InvoiceResponse> CreateInvoice([FromBody] CreateInvoiceRequest request)
            => Ok(_service.CreateInvoice(request));

        [HttpGet("invoices")]
        public ActionResult GetInvoices([FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (items, total) = _service.GetInvoices(status, page, pageSize);
            return Ok(new { items, total, page, pageSize });
        }

        [HttpGet("invoices/{id:long}")]
        public ActionResult<InvoiceResponse> GetInvoice(long id)
        {
            var invoice = _service.GetInvoiceById(id);
            return invoice == null ? NotFound() : Ok(invoice);
        }

        [HttpPatch("invoices/{id:long}/status")]
        public ActionResult UpdateInvoiceStatus(long id, [FromBody] UpdateInvoiceStatusRequest request)
            => _service.UpdateInvoiceStatus(id, request) ? NoContent() : NotFound();

        // ─────────────────────────────────────────────
        // AGENT COMMISSIONS
        // ─────────────────────────────────────────────

        [HttpPost("commissions")]
        public ActionResult<CommissionResponse> CreateCommission([FromBody] CreateCommissionRequest request)
            => Ok(_service.CreateCommission(request));

        [HttpGet("commissions")]
        public ActionResult<IEnumerable<CommissionResponse>> GetCommissions([FromQuery] string? status)
            => Ok(_service.GetCommissions(status));

        [HttpGet("commissions/{id:long}")]
        public ActionResult<CommissionResponse> GetCommission(long id)
        {
            var c = _service.GetCommissionById(id);
            return c == null ? NotFound() : Ok(c);
        }

        [HttpGet("commissions/booking/{bookingId:long}")]
        public ActionResult<IEnumerable<CommissionResponse>> GetCommissionsByBooking(long bookingId)
            => Ok(_service.GetCommissionsByBooking(bookingId));

        [HttpPatch("commissions/{id:long}/status")]
        public ActionResult UpdateCommissionStatus(long id, [FromBody] UpdateCommissionStatusRequest request)
            => _service.UpdateCommissionStatus(id, request) ? NoContent() : NotFound();

        // ─────────────────────────────────────────────
        // EXPENSES
        // ─────────────────────────────────────────────

        [HttpPost("expenses")]
        public ActionResult<ExpenseResponse> CreateExpense([FromBody] CreateExpenseRequest request)
            => Ok(_service.CreateExpense(request));

        [HttpGet("expenses")]
        public ActionResult<IEnumerable<ExpenseResponse>> GetExpenses([FromQuery] long? bookingId, [FromQuery] string? category)
            => Ok(_service.GetExpenses(bookingId, category));

        [HttpGet("expenses/{id:long}")]
        public ActionResult<ExpenseResponse> GetExpense(long id)
        {
            var e = _service.GetExpenseById(id);
            return e == null ? NotFound() : Ok(e);
        }

        [HttpPut("expenses/{id:long}")]
        public ActionResult UpdateExpense(long id, [FromBody] UpdateExpenseRequest request)
            => _service.UpdateExpense(id, request) ? NoContent() : NotFound();

        [HttpDelete("expenses/{id:long}")]
        public ActionResult DeleteExpense(long id)
            => _service.DeleteExpense(id) ? NoContent() : NotFound();

        // ─────────────────────────────────────────────
        // REFUNDS
        // ─────────────────────────────────────────────

        [HttpPost("refunds")]
        public ActionResult<RefundResponse> CreateRefund([FromBody] CreateRefundRequest request)
            => Ok(_service.CreateRefund(request));

        [HttpGet("refunds")]
        public ActionResult<IEnumerable<RefundResponse>> GetRefunds([FromQuery] string? status)
            => Ok(_service.GetRefunds(status));

        [HttpGet("refunds/{id:long}")]
        public ActionResult<RefundResponse> GetRefund(long id)
        {
            var r = _service.GetRefundById(id);
            return r == null ? NotFound() : Ok(r);
        }

        [HttpPatch("refunds/{id:long}/process")]
        public ActionResult ProcessRefund(long id, [FromBody] ProcessRefundRequest request)
        {
            var processedBy = User.Identity?.Name ?? "system";
            return _service.ProcessRefund(id, request, processedBy) ? NoContent() : NotFound();
        }
    }
}
