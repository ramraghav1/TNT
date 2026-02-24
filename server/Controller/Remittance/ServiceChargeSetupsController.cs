using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.ServiceChargeSetup;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/service-charges")]
    public class ServiceChargeSetupsController : ControllerBase
    {
        private readonly IServiceChargeSetupService _service;

        public ServiceChargeSetupsController(IServiceChargeSetupService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateSetupRequest request)
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
        public IActionResult Update(long id, [FromBody] UpdateSetupRequest request)
        {
            var result = _service.Update(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            return _service.Delete(id) ? NoContent() : NotFound();
        }

        // Calculate service charge for a given amount + corridor
        [HttpPost("calculate")]
        public IActionResult CalculateCharge([FromBody] CalculateChargeRequest request)
        {
            var result = _service.CalculateCharge(request);
            return result == null
                ? NotFound(new { message = "No matching service charge setup found for this corridor/amount." })
                : Ok(result);
        }
    }
}
