using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Clinic;
using static Domain.Models.Clinic.Practitioner;

namespace server.Controller.Clinic
{
    [ApiController]
    [Route("api/clinic/tenants/{tenantId}/practitioners")]
    public class PractitionersController : ControllerBase
    {
        private readonly IPractitionerService _service;

        public PractitionersController(IPractitionerService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create(long tenantId, [FromBody] CreatePractitionerRequest request)
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

        [HttpGet("{id}")]
        public IActionResult GetById(long tenantId, long id)
        {
            var result = _service.GetById(tenantId, id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long tenantId, long id, [FromBody] UpdatePractitionerRequest request)
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
