using Microsoft.AspNetCore.Mvc;
using static Domain.Models.TourAndTravels.Itinerary;
using Bussiness.Services.TourAndTravels;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItinerariesController : ControllerBase
    {
        private readonly IItineraryService _itineraryService;

        public ItinerariesController(IItineraryService itineraryService)
        {
            _itineraryService = itineraryService;
        }

        // ============================================================
        // 1️⃣ CREATE ITINERARY
        [Route("create")]
        [HttpPost]
        public IActionResult CreateItinerary([FromBody] CreateItineraryRequest request)
        {
            var result = _itineraryService.CreateItinerary(request);

            if (result != null && result.Id > 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        // ============================================================
        // 2️⃣ GET ALL ITINERARIES
        [Route("list")]
        [HttpGet]
        public IActionResult GetAllItineraries()
        {
            var result = _itineraryService.GetAllItineraries();
            return Ok(result);
        }

        // ============================================================
        // 3️⃣ GET BY ID
        [Route("detail")]
        [HttpGet]
        public IActionResult GetItineraryById(long id)
        {
            var result = _itineraryService.GetItineraryById(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ============================================================
        // 4️⃣ UPDATE
        [Route("update")]
        [HttpPost]
        public IActionResult UpdateItinerary(long id, [FromBody] UpdateItineraryRequest request)
        {
            var result = _itineraryService.UpdateItinerary(id, request);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ============================================================
        // 5️⃣ DELETE
        [Route("delete")]
        [HttpPost]
        public IActionResult DeleteItinerary(long id)
        {
            var success = _itineraryService.DeleteItinerary(id);

            if (!success)
                return NotFound();

            return Ok();
        }
    }
}