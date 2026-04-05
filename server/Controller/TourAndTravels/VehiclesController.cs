using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Inventory;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/inventory/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // POST /api/inventory/vehicles
        [HttpPost]
        public IActionResult CreateVehicle([FromBody] CreateVehicleRequest request)
        {
            // TODO: Get actual user ID from authentication
            long userId = 1;
            
            var result = _vehicleService.CreateVehicle(request, userId);
            return Ok(result);
        }

        // GET /api/inventory/vehicles
        [HttpGet]
        public IActionResult GetAllVehicles([FromQuery] bool includeInactive = false)
        {
            var result = _vehicleService.GetAllVehicles(includeInactive);
            return Ok(result);
        }

        // GET /api/inventory/vehicles/{id}
        [HttpGet("{id:long}")]
        public IActionResult GetVehicleById(long id)
        {
            var result = _vehicleService.GetVehicleById(id);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // PUT /api/inventory/vehicles/{id}
        [HttpPut("{id:long}")]
        public IActionResult UpdateVehicle(long id, [FromBody] UpdateVehicleRequest request)
        {
            var result = _vehicleService.UpdateVehicle(id, request);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // DELETE /api/inventory/vehicles/{id}
        [HttpDelete("{id:long}")]
        public IActionResult DeleteVehicle(long id)
        {
            var success = _vehicleService.DeleteVehicle(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Vehicle deactivated successfully" });
        }

        // POST /api/inventory/vehicles/{id}/activate
        [HttpPost("{id:long}/activate")]
        public IActionResult ActivateVehicle(long id)
        {
            var success = _vehicleService.ActivateVehicle(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Vehicle activated successfully" });
        }
    }
}
