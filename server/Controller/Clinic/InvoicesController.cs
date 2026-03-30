using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Clinic;
using static Domain.Models.Clinic.Invoice;

namespace server.Controller.Clinic
{
    [ApiController]
    [Route("api/clinic/tenants/{tenantId}/invoices")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoicesController(IInvoiceService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create(long tenantId, [FromBody] CreateInvoiceRequest request)
        {
            request.TenantId = tenantId;
            var result = _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { tenantId, id = result.Id }, result);
        }

        [HttpGet]
        public IActionResult GetByTenant(long tenantId)
        {
            return Ok(_service.GetByTenant(tenantId));
        }

        [HttpGet("by-patient/{patientId}")]
        public IActionResult GetByPatient(long tenantId, long patientId)
        {
            return Ok(_service.GetByPatient(tenantId, patientId));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long tenantId, long id)
        {
            var result = _service.GetById(tenantId, id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long tenantId, long id, [FromBody] UpdateInvoiceRequest request)
        {
            var result = _service.Update(tenantId, id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long tenantId, long id)
        {
            return _service.Delete(tenantId, id) ? NoContent() : NotFound();
        }
    }
}
