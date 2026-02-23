using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Booking;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ===========================
        // Create a new booking from template OR reuse existing instance
        // POST /api/bookings/create
        // Body: { itineraryId, sourceInstanceId?, startDate, endDate, travelers[], specialRequests? }
        // ===========================
        [HttpPost("create")]
        public ActionResult<BookingResponse> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var booking = _bookingService.CreateBooking(request);
            return Ok(booking);
        }

        // ===========================
        // Get all bookings (lightweight list)
        // GET /api/bookings
        // ===========================
        [HttpGet]
        public ActionResult<List<BookingListItem>> GetAllBookings()
        {
            var bookings = _bookingService.GetAllBookings();
            return Ok(bookings);
        }

        // ===========================
        // Get full booking detail by instance ID
        // GET /api/bookings/{id}
        // ===========================
        [HttpGet("{id:long}")]
        public ActionResult<BookingDetailResponse> GetBookingById(long id)
        {
            var booking = _bookingService.GetBookingById(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        // ===========================
        // Customize a single day of the booking instance
        // Only updates that specific day — other days remain intact
        // PUT /api/bookings/{id}/customize-day
        // Body: { instanceDayId, title?, location?, accommodation?, transport?, meals?, activities? }
        // ===========================
        [HttpPut("{id:long}/customize-day")]
        public ActionResult<BookingDayResponse> CustomizeDay(long id, [FromBody] CustomizeDayRequest request)
        {
            var day = _bookingService.CustomizeDay(id, request);
            if (day == null) return NotFound("Day not found for this booking instance.");
            return Ok(day);
        }

        // ===========================
        // Approve booking (traveler or admin)
        // POST /api/bookings/{id}/approve
        // Body: { approvedBy: "traveler"|"admin", approved: true/false, remarks? }
        // ===========================
        [HttpPost("{id:long}/approve")]
        public IActionResult ApproveBooking(long id, [FromBody] ApproveBookingRequest request)
        {
            var result = _bookingService.ApproveBooking(id, request);
            if (!result) return NotFound();
            return Ok(new { message = "Approval recorded successfully." });
        }

        // ===========================
        // Add payment to booking
        // POST /api/bookings/{id}/payment
        // Body: { amount, currency?, paymentMethod?, transactionReference? }
        // ===========================
        [HttpPost("{id:long}/payment")]
        public ActionResult<PaymentResponse> AddPayment(long id, [FromBody] AddPaymentRequest request)
        {
            var payment = _bookingService.AddPayment(id, request);
            return Ok(payment);
        }

        // ===========================
        // Get all payments for a booking
        // GET /api/bookings/{id}/payments
        // ===========================
        [HttpGet("{id:long}/payments")]
        public ActionResult<List<PaymentResponse>> GetPayments(long id)
        {
            var payments = _bookingService.GetPayments(id);
            return Ok(payments);
        }

        // ===========================
        // Update booking status
        // PUT /api/bookings/{id}/status
        // Body: { status: "Draft"|"Pending"|"Confirmed"|"Cancelled" }
        // ===========================
        [HttpPut("{id:long}/status")]
        public IActionResult UpdateStatus(long id, [FromBody] UpdateStatusRequest request)
        {
            var result = _bookingService.UpdateStatus(id, request.Status);
            if (!result) return NotFound();
            return Ok(new { message = $"Status updated to {request.Status}." });
        }
    }

    // Helper class for status update endpoint
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}