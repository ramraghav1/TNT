using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.DepartureManagement;

namespace server.Controller.TourAndTravels
{
    /// <summary>
    /// Manage Bookings → Departure creation and assignment.
    /// Base route: POST /api/departure-management
    /// </summary>
    [ApiController]
    [Route("api/departure-management")]
    public class DepartureManagementController : ControllerBase
    {
        private readonly IDepartureManagementService _service;

        public DepartureManagementController(IDepartureManagementService service)
        {
            _service = service;
        }

        // ────────────────────────────────────────────────────────────
        // GET  /api/departure-management/unassigned-bookings
        // Returns bookings with departure_id IS NULL, grouped by itinerary
        // ────────────────────────────────────────────────────────────
        [HttpGet("unassigned-bookings")]
        public ActionResult<List<BookingGroupByItinerary>> GetUnassignedBookings()
        {
            var result = _service.GetUnassignedBookingsGrouped();
            return Ok(result);
        }

        // ────────────────────────────────────────────────────────────
        // GET  /api/departure-management/suggest?itineraryId=1&requiredSeats=5
        // Returns existing departures that have capacity for the seats
        // ────────────────────────────────────────────────────────────
        [HttpGet("suggest")]
        public ActionResult<List<SuggestedDepartureItem>> SuggestDepartures(
            [FromQuery] long itineraryId,
            [FromQuery] int requiredSeats)
        {
            var result = _service.GetSuggestedDepartures(itineraryId, requiredSeats);
            return Ok(result);
        }

        // ────────────────────────────────────────────────────────────
        // POST  /api/departure-management/create
        // Creates a new departure and assigns selected bookings
        // ────────────────────────────────────────────────────────────
        [HttpPost("create")]
        public ActionResult<DepartureDetailResponse> CreateDeparture([FromBody] CreateDepartureRequest request)
        {
            try
            {
                var result = _service.CreateDeparture(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────────────────
        // POST  /api/departure-management/assign
        // Assigns selected bookings to an existing departure
        // ────────────────────────────────────────────────────────────
        [HttpPost("assign")]
        public ActionResult<DepartureDetailResponse> AssignToDeparture([FromBody] AssignBookingsToDepartureRequest request)
        {
            try
            {
                var result = _service.AssignBookingsToDeparture(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ────────────────────────────────────────────────────────────
        // GET  /api/departure-management/{id}
        // Returns departure detail with assigned bookings
        // ────────────────────────────────────────────────────────────
        [HttpGet("{id:long}")]
        public ActionResult<DepartureDetailResponse> GetDeparture(long id)
        {
            var result = _service.GetDepartureById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ────────────────────────────────────────────────────────────
        // GET  /api/departure-management/all
        // Returns all departures with guide/vehicle/traveler info + computed status
        // ────────────────────────────────────────────────────────────
        [HttpGet("all")]
        public ActionResult<List<DepartureListItem>> GetAllDepartures()
        {
            return Ok(_service.GetAllDepartures());
        }
    }
}
