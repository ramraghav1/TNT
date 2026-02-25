using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.AgentAccount;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/agent-accounts")]
    public class AgentAccountsController : ControllerBase
    {
        private readonly IAgentAccountService _service;

        public AgentAccountsController(IAgentAccountService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateAgentAccountRequest request)
        {
            var result = _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("by-agent/{agentId}")]
        public IActionResult GetByAgent(long agentId)
        {
            return Ok(_service.GetByAgent(agentId));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var result = _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-agent/{agentId}/currency/{currencyCode}")]
        public IActionResult GetByAgentAndCurrency(long agentId, string currencyCode)
        {
            var result = _service.GetByAgentAndCurrency(agentId, currencyCode);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateAgentAccountRequest request)
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
