using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Inventory;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/inventory/[controller]")]
    public class GuidesController : ControllerBase
    {
        private readonly IGuideService _guideService;

        public GuidesController(IGuideService guideService)
        {
            _guideService = guideService;
        }

        // POST /api/inventory/guides
        [HttpPost]
        public IActionResult CreateGuide([FromBody] CreateGuideRequest request)
        {
            // TODO: Get actual user ID from authentication
            long userId = 1;
            
            var result = _guideService.CreateGuide(request, userId);
            return Ok(result);
        }

        // GET /api/inventory/guides
        [HttpGet]
        public IActionResult GetAllGuides([FromQuery] bool includeInactive = false)
        {
            var result = _guideService.GetAllGuides(includeInactive);
            return Ok(result);
        }

        // GET /api/inventory/guides/{id}
        [HttpGet("{id:long}")]
        public IActionResult GetGuideById(long id)
        {
            var result = _guideService.GetGuideById(id);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // PUT /api/inventory/guides/{id}
        [HttpPut("{id:long}")]
        public IActionResult UpdateGuide(long id, [FromBody] UpdateGuideRequest request)
        {
            var result = _guideService.UpdateGuide(id, request);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // DELETE /api/inventory/guides/{id}
        [HttpDelete("{id:long}")]
        public IActionResult DeleteGuide(long id)
        {
            var success = _guideService.DeleteGuide(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Guide deactivated successfully" });
        }

        // POST /api/inventory/guides/{id}/activate
        [HttpPost("{id:long}/activate")]
        public IActionResult ActivateGuide(long id)
        {
            var success = _guideService.ActivateGuide(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Guide activated successfully" });
        }
    }
}
