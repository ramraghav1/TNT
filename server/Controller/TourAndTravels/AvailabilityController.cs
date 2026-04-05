using System;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Availability;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        // ===========================
        // AVAILABILITY CHECKING
        // ===========================

        // POST /api/availability/check
        [HttpPost("check")]
        public IActionResult CheckAvailability([FromBody] CheckAvailabilityRequest request)
        {
            var result = _availabilityService.CheckAvailability(request);
            return Ok(result);
        }

        // POST /api/availability/initialize
        [HttpPost("initialize")]
        public IActionResult InitializeAvailability([FromBody] InitializeAvailabilityRequest request)
        {
            var success = _availabilityService.InitializeAvailability(
                request.InventoryType,
                request.InventoryId,
                request.StartDate,
                request.EndDate,
                request.Capacity
            );
            
            return Ok(new { message = "Availability initialized successfully" });
        }

        // ===========================
        // AVAILABILITY BLOCKS
        // ===========================

        // POST /api/availability/blocks
        [HttpPost("blocks")]
        public IActionResult CreateBlock([FromBody] BlockAvailabilityRequest request)
        {
            // TODO: Get actual user ID from authentication
            long userId = 1;
            
            var result = _availabilityService.CreateBlock(request, userId);
            return Ok(result);
        }

        // GET /api/availability/blocks
        [HttpGet("blocks")]
        public IActionResult GetBlocks([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = _availabilityService.GetBlocks(startDate, endDate);
            return Ok(result);
        }

        // DELETE /api/availability/blocks/{id}
        [HttpDelete("blocks/{id:long}")]
        public IActionResult DeleteBlock(long id)
        {
            var success = _availabilityService.DeleteBlock(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Block removed successfully" });
        }

        // ===========================
        // CALENDAR VIEW
        // ===========================

        // POST /api/availability/calendar
        [HttpPost("calendar")]
        public IActionResult GetCalendarView([FromBody] CalendarViewRequest request)
        {
            var result = _availabilityService.GetCalendarView(request);
            return Ok(result);
        }

        // ===========================
        // PACKAGE DEPARTURES
        // ===========================

        // POST /api/availability/departures
        [HttpPost("departures")]
        public IActionResult CreatePackageDeparture([FromBody] CreatePackageDepartureRequest request)
        {
            var result = _availabilityService.CreatePackageDeparture(request);
            return Ok(result);
        }

        // GET /api/availability/departures
        [HttpGet("departures")]
        public IActionResult GetPackageDepartures([FromQuery] long? itineraryId = null)
        {
            var result = _availabilityService.GetPackageDepartures(itineraryId);
            return Ok(result);
        }

        // GET /api/availability/departures/{id}
        [HttpGet("departures/{id:long}")]
        public IActionResult GetPackageDepartureById(long id)
        {
            var result = _availabilityService.GetPackageDepartureById(id);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // PUT /api/availability/departures/{id}
        [HttpPut("departures/{id:long}")]
        public IActionResult UpdatePackageDeparture(long id, [FromBody] UpdatePackageDepartureRequest request)
        {
            var result = _availabilityService.UpdatePackageDeparture(id, request);
            
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // DELETE /api/availability/departures/{id}
        [HttpDelete("departures/{id:long}")]
        public IActionResult DeletePackageDeparture(long id)
        {
            var success = _availabilityService.DeletePackageDeparture(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Package departure deleted successfully" });
        }

        // ===========================
        // BOOKING INVENTORY LINKS
        // ===========================

        // POST /api/availability/booking-inventory
        [HttpPost("booking-inventory")]
        public IActionResult AssignInventoryToBooking([FromBody] AssignInventoryToBookingRequest request)
        {
            var result = _availabilityService.AssignInventoryToBooking(request);
            return Ok(result);
        }

        // GET /api/availability/booking-inventory/{bookingInstanceId}
        [HttpGet("booking-inventory/{bookingInstanceId:long}")]
        public IActionResult GetBookingInventory(long bookingInstanceId)
        {
            var result = _availabilityService.GetBookingInventory(bookingInstanceId);
            return Ok(result);
        }

        // DELETE /api/availability/booking-inventory/{id}
        [HttpDelete("booking-inventory/{id:long}")]
        public IActionResult RemoveInventoryFromBooking(long id)
        {
            var success = _availabilityService.RemoveInventoryFromBooking(id);
            
            if (!success)
                return NotFound();
            
            return Ok(new { message = "Inventory removed from booking successfully" });
        }
    }

    // Helper class for initialize endpoint
    public class InitializeAvailabilityRequest
    {
        public string InventoryType { get; set; } = string.Empty;
        public long InventoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
    }
}
