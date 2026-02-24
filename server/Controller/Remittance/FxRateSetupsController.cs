using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.FxRateSetup;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/fx-rates")]
    public class FxRateSetupsController : ControllerBase
    {
        private readonly IFxRateSetupService _service;

        public FxRateSetupsController(IFxRateSetupService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateFxRateRequest request)
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
        public IActionResult Update(long id, [FromBody] UpdateFxRateRequest request)
        {
            var result = _service.Update(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            return _service.Delete(id) ? NoContent() : NotFound();
        }

        /// <summary>
        /// Convert an amount using the active FX rate for a corridor.
        /// </summary>
        [HttpPost("convert")]
        public IActionResult Convert([FromBody] ConvertRequest request)
        {
            var result = _service.Convert(request);
            return result == null
                ? NotFound(new { message = "No matching FX rate found for this corridor." })
                : Ok(result);
        }
    }
}
