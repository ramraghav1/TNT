using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Inventory;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/inventory/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // POST /api/inventory/hotels
        [HttpPost]
        public IActionResult CreateHotel([FromBody] CreateHotelRequest request)
        {
            // TODO: Get actual user ID from authentication
            long userId = 1;
            
            var result = _hotelService.CreateHotel(request, userId);
            return Ok(result);
        }

        // GET /api/inventory/hotels
        [HttpGet]
        public IActionResult GetAllHotels([FromQuery] bool includeInactive = false)
        {
            var result = _hotelService.GetAllHotels(includeInactive);
            return Ok(result);
        }

        // GET /api/inventory/hotels/{id}
        [HttpGet("{id:long}")]
        public IActionResult GetHotelById(long id)
        {
            var result = _hotelService.GetHotelById(id);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // PUT /api/inventory/hotels/{id}
        [HttpPut("{id:long}")]
        public IActionResult UpdateHotel(long id, [FromBody] UpdateHotelRequest request)
        {
            var result = _hotelService.UpdateHotel(id, request);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // DELETE /api/inventory/hotels/{id}
        [HttpDelete("{id:long}")]
        public IActionResult DeleteHotel(long id)
        {
            var success = _hotelService.DeleteHotel(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Hotel deactivated successfully" });
        }

        // POST /api/inventory/hotels/{id}/activate
        [HttpPost("{id:long}/activate")]
        public IActionResult ActivateHotel(long id)
        {
            var success = _hotelService.ActivateHotel(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Hotel activated successfully" });
        }
    }
}
