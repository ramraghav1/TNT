using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Domain.Models.TourAndTravels; // ✅ Your DTOs/models namespace
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
        [HttpPost]
        public ActionResult<ItineraryResponse> CreateItinerary([FromBody] CreateItineraryRequest request)
        {
            var created = _itineraryService.CreateItinerary(request);
            return CreatedAtAction(nameof(GetItineraryById), new { id = created.Id }, created);
        }

        // ============================================================
        // 2️⃣ GET ALL ITINERARIES
        [HttpGet]
        public ActionResult<List<ItineraryResponse>> GetAllItineraries()
        {
            var itineraries = _itineraryService.GetAllItineraries();
            return Ok(itineraries);
        }

        // ============================================================
        // 3️⃣ GET ITINERARY BY ID (DETAIL)
        [HttpGet("{id:long}")]
        public ActionResult<ItineraryDetailResponse> GetItineraryById(long id)
        {
            var itinerary = _itineraryService.GetItineraryById(id);
            if (itinerary == null)
                return NotFound();
            return Ok(itinerary);
        }

        // ============================================================
        // 4️⃣ UPDATE ITINERARY
        [HttpPut("{id:long}")]
        public ActionResult<ItineraryResponse> UpdateItinerary(long id, UpdateItineraryRequest request)
        {
            var updated = _itineraryService.UpdateItinerary(id, request);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        // ============================================================
        // 5️⃣ DELETE ITINERARY
        [HttpDelete("{id:long}")]
        public IActionResult DeleteItinerary(long id)
        {
            var success = _itineraryService.DeleteItinerary(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        // ============================================================
        // 6️⃣ ADD DAY TO ITINERARY
        [HttpPost("{id:long}/days")]
        public ActionResult<ItineraryDayResponse> AddDay(long id, [FromBody] CreateItineraryDayRequest request)
        {
            var day = _itineraryService.AddDayToItinerary(id, request);
            if (day == null)
                return NotFound();
            return Ok(day);
        }

        // ============================================================
        // 7️⃣ UPDATE DAY
        [HttpPut("{id:long}/days/{dayId:long}")]
        public ActionResult<ItineraryDayResponse> UpdateDay(long id, long dayId, [FromBody] UpdateItineraryDayRequest request)
        {
            var day = _itineraryService.UpdateDay(id, dayId, request);
            if (day == null)
                return NotFound();
            return Ok(day);
        }
    }
}