using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Clinic;
using static Domain.Models.Clinic.Tenant;

namespace server.Controller.Clinic
{
    [ApiController]
    [Route("api/clinic/tenants")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _service;

        public TenantsController(ITenantService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateTenantRequest request)
        {
            var result = _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var result = _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-slug/{slug}")]
        public IActionResult GetBySlug(string slug)
        {
            var result = _service.GetBySlug(slug);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateTenantRequest request)
        {
            var result = _service.Update(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            return _service.Delete(id) ? NoContent() : NotFound();
        }
    }
}
