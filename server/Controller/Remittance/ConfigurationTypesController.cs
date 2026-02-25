using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.ConfigurationType;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/configuration-types")]
    public class ConfigurationTypesController : ControllerBase
    {
        private readonly IConfigurationTypeService _service;

        public ConfigurationTypesController(IConfigurationTypeService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateConfigurationTypeRequest request)
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

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateConfigurationTypeRequest request)
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
