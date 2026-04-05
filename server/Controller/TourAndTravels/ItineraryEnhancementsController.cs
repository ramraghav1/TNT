using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.ItineraryEnhancements;
using System.Collections.Generic;

namespace server.Controller.TourAndTravels
{
    /// <summary>
    /// API endpoints for itinerary media, assignments, availability checking, and booking requests
    /// </summary>
    [ApiController]
    [Route("api/inventory/itinerary-enhancements")]
    public class ItineraryEnhancementsController : ControllerBase
    {
        private readonly IItineraryEnhancementsService _service;

        public ItineraryEnhancementsController(IItineraryEnhancementsService service)
        {
            _service = service;
        }

        // ============================================================
        // MEDIA MANAGEMENT
        // ============================================================

        /// <summary>
        /// Add media (photo, video, video link) to an itinerary
        /// </summary>
        [HttpPost("itineraries/{itineraryId}/media")]
        [Authorize]
        public IActionResult AddMedia(long itineraryId, [FromBody] CreateMediaRequest request)
        {
            try
            {
                var mediaId = _service.AddMedia(itineraryId, request);
                return Ok(new { mediaId, message = "Media added successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all media for an itinerary
        /// </summary>
        [HttpGet("itineraries/{itineraryId}/media")]
        public IActionResult GetMedia(long itineraryId)
        {
            var media = _service.GetMedia(itineraryId);
            return Ok(media);
        }

        /// <summary>
        /// Delete a media item
        /// </summary>
        [HttpDelete("media/{mediaId}")]
        [Authorize]
        public IActionResult DeleteMedia(long mediaId)
        {
            var success = _service.DeleteMedia(mediaId);
            return success 
                ? Ok(new { message = "Media deleted successfully" })
                : NotFound(new { message = "Media not found" });
        }

        /// <summary>
        /// Reorder media items for an itinerary
        /// </summary>
        [HttpPut("itineraries/{itineraryId}/media/reorder")]
        [Authorize]
        public IActionResult ReorderMedia(long itineraryId, [FromBody] List<long> mediaIds)
        {
            try
            {
                _service.ReorderMedia(itineraryId, mediaIds);
                return Ok(new { message = "Media reordered successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================
        // ASSIGNMENTS
        // ============================================================

        /// <summary>
        /// Create a booking with hotel/guide/vehicle assignments
        /// </summary>
        [HttpPost("bookings/create-with-assignments")]
        [Authorize]
        public IActionResult CreateBookingWithAssignments([FromBody] CreateBookingWithAssignmentsRequest request)
        {
            try
            {
                var response = _service.CreateBookingWithAssignments(request);
                
                if (response.Conflicts.Any())
                {
                    return Ok(new 
                    { 
                        status = "ConflictsDetected",
                        message = "Some resources have conflicts",
                        conflicts = response.Conflicts 
                    });
                }

                return Ok(new
                {
                    status = "Success",
                    instanceId = response.InstanceId,
                    bookingReference = response.BookingReference,
                    message = "Booking created successfully"
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add assignment to an existing booking day
        /// </summary>
        [HttpPost("assignments")]
        [Authorize]
        public IActionResult CreateAssignment([FromBody] CreateAssignmentRequest request)
        {
            try
            {
                var assignmentId = _service.CreateAssignment(request);
                return Ok(new { assignmentId, message = "Assignment created successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all assignments for a booking instance
        /// </summary>
        [HttpGet("instances/{instanceId}/assignments")]
        [Authorize]
        public IActionResult GetAssignments(long instanceId)
        {
            var assignments = _service.GetAssignmentsForInstance(instanceId);
            return Ok(assignments);
        }

        /// <summary>
        /// Update an assignment
        /// </summary>
        [HttpPut("assignments/{assignmentId}")]
        [Authorize]
        public IActionResult UpdateAssignment(long assignmentId, [FromBody] CreateAssignmentRequest request)
        {
            try
            {
                var success = _service.UpdateAssignment(assignmentId, request);
                return success
                    ? Ok(new { message = "Assignment updated successfully" })
                    : NotFound(new { message = "Assignment not found" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an assignment
        /// </summary>
        [HttpDelete("assignments/{assignmentId}")]
        [Authorize]
        public IActionResult DeleteAssignment(long assignmentId)
        {
            var success = _service.DeleteAssignment(assignmentId);
            return success
                ? Ok(new { message = "Assignment deleted successfully" })
                : NotFound(new { message = "Assignment not found" });
        }

        // ============================================================
        // AVAILABILITY CHECKING
        // ============================================================

        /// <summary>
        /// Check availability of hotels, guides, and vehicles for a date range
        /// </summary>
        [HttpPost("check-availability")]
        [Authorize]
        public IActionResult CheckAvailability([FromBody] CheckAvailabilityRequest request)
        {
            try
            {
                var response = _service.CheckAvailability(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Validate assignments for conflicts before creating booking
        /// </summary>
        [HttpPost("instances/{instanceId}/validate-assignments")]
        [Authorize]
        public IActionResult ValidateAssignments(long instanceId, [FromBody] List<DayAssignmentInput> assignments)
        {
            try
            {
                var conflicts = _service.ValidateAssignments(instanceId, assignments);
                
                if (conflicts.Any())
                {
                    return Ok(new
                    {
                        hasConflicts = true,
                        conflicts = conflicts
                    });
                }

                return Ok(new
                {
                    hasConflicts = false,
                    message = "All assignments are available"
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================
        // BOOKING REQUESTS (from website)
        // ============================================================

        /// <summary>
        /// Create a booking request from the public website
        /// </summary>
        [HttpPost("booking-requests")]
        [AllowAnonymous]
        public IActionResult CreateBookingRequest([FromBody] CreateBookingRequestRequest request)
        {
            try
            {
                var requestId = _service.CreateBookingRequest(request);
                return Ok(new 
                { 
                    requestId, 
                    message = "Your booking request has been submitted. We'll contact you soon!" 
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all booking requests (optionally filtered by status)
        /// </summary>
        [HttpGet("booking-requests")]
        [Authorize]
        public IActionResult GetBookingRequests([FromQuery] string? status = null)
        {
            var requests = _service.GetBookingRequests(status);
            return Ok(requests);
        }

        /// <summary>
        /// Get a specific booking request
        /// </summary>
        [HttpGet("booking-requests/{id}")]
        [Authorize]
        public IActionResult GetBookingRequest(long id)
        {
            var request = _service.GetBookingRequest(id);
            return request != null
                ? Ok(request)
                : NotFound(new { message = "Booking request not found" });
        }

        /// <summary>
        /// Update booking request status
        /// </summary>
        [HttpPut("booking-requests/{id}/status")]
        [Authorize]
        public IActionResult UpdateBookingRequestStatus(
            long id, 
            [FromBody] BookingRequestStatusUpdate request)
        {
            try
            {
                var username = User.Identity?.Name ?? "System";
                var success = _service.RespondToBookingRequest(id, request.Status, username);
                return success
                    ? Ok(new { message = "Status updated successfully" })
                    : NotFound(new { message = "Booking request not found" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Convert a booking request to an actual booking instance
        /// </summary>
        [HttpPost("booking-requests/{requestId}/convert")]
        [Authorize]
        public IActionResult ConvertBookingRequest(
            long requestId, 
            [FromBody] CreateBookingWithAssignmentsRequest bookingRequest)
        {
            try
            {
                var instanceId = _service.ConvertBookingRequestToInstance(requestId, bookingRequest);
                return Ok(new
                {
                    instanceId,
                    message = "Booking request converted successfully"
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ============================================================
        // PUBLIC API (for website)
        // ============================================================

        /// <summary>
        /// Get published itineraries for public website
        /// </summary>
        [HttpGet("public/itineraries")]
        [AllowAnonymous]
        public IActionResult GetPublishedItineraries()
        {
            var itineraries = _service.GetPublishedItineraries();
            return Ok(itineraries);
        }

        /// <summary>
        /// Get full details of a published itinerary (for public website)
        /// </summary>
        [HttpGet("public/itineraries/{itineraryId}")]
        [AllowAnonymous]
        public IActionResult GetPublicItinerary(long itineraryId)
        {
            var itinerary = _service.GetPublicItinerary(itineraryId);
            return itinerary != null
                ? Ok(itinerary)
                : NotFound(new { message = "Itinerary not found or not published" });
        }
    }

    // ============================================================
    // REQUEST DTOs
    // ============================================================

    public class BookingRequestStatusUpdate
    {
        public string Status { get; set; } = string.Empty;
    }
}
