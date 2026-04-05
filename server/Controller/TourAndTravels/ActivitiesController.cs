using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Inventory;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/inventory/activities")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // POST /api/inventory/activities
        [HttpPost]
        public IActionResult CreateActivity([FromBody] CreateActivityRequest request)
        {
            // TODO: Get actual user ID from authentication
            long userId = 1;
            
            var result = _activityService.CreateActivity(request, userId);
            return Ok(result);
        }

        // GET /api/inventory/activities
        [HttpGet]
        public IActionResult GetAllActivities([FromQuery] bool includeInactive = false)
        {
            var result = _activityService.GetAllActivities(includeInactive);
            return Ok(result);
        }

        // GET /api/inventory/activities/{id}
        [HttpGet("{id:long}")]
        public IActionResult GetActivityById(long id)
        {
            var result = _activityService.GetActivityById(id);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // PUT /api/inventory/activities/{id}
        [HttpPut("{id:long}")]
        public IActionResult UpdateActivity(long id, [FromBody] UpdateActivityRequest request)
        {
            var result = _activityService.UpdateActivity(id, request);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // DELETE /api/inventory/activities/{id}
        [HttpDelete("{id:long}")]
        public IActionResult DeleteActivity(long id)
        {
            var success = _activityService.DeleteActivity(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Activity deactivated successfully" });
        }

        // POST /api/inventory/activities/{id}/activate
        [HttpPost("{id:long}/activate")]
        public IActionResult ActivateActivity(long id)
        {
            var success = _activityService.ActivateActivity(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Activity activated successfully" });
        }
    }
}
