using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.Agent;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/agents")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _service;

        public AgentsController(IAgentService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateAgentRequest request)
        {
            var result = _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("by-country/{countryId}")]
        public IActionResult GetByCountry(long countryId)
        {
            return Ok(_service.GetByCountry(countryId));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var result = _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateAgentRequest request)
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
