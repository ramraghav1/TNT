using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using Domain.Models.TourAndTravels;
using static Domain.Models.TourAndTravels.Booking;
using static Domain.Models.TourAndTravels.Payments;

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
        // Create a new booking from an itinerary
        // ===========================
        [HttpPost("create")]
        public ActionResult<BookingResponse> CreateBooking([FromBody] CreateBookingRequest request)
        {
            var booking = _bookingService.CreateBooking(request);
            return Ok(booking);
        }

        // ===========================
        // Get all bookings
        // ===========================
        [HttpGet]
        public ActionResult<List<BookingResponse>> GetAllBookings()
        {
            var bookings = _bookingService.GetAllBookings();
            return Ok(bookings);
        }

        // ===========================
        // Get a specific booking by Id
        // ===========================
        [HttpGet("{id:long}")]
        public ActionResult<BookingDetailResponse?> GetBookingById(long id)
        {
            var booking = _bookingService.GetBookingById(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        // ===========================
        // Update booking (customize)
        // ===========================
        [HttpPut("{id:long}/update")]
        public ActionResult<BookingResponse?> UpdateBooking(long id, [FromBody] UpdateBookingRequest request)
        {
            var updatedBooking = _bookingService.UpdateBooking(id, request);
            if (updatedBooking == null) return NotFound();
            return Ok(updatedBooking);
        }

        // ===========================
        // Approve booking (traveler)
        // ===========================
        [HttpPost("{id:long}/approve/traveler")]
        public IActionResult ApproveBookingByTraveler(long id)
        {
            //_bookingService.ApproveBookingByTraveler(id);
            return Ok();
        }

        // ===========================
        // Approve booking (admin)
        // ===========================
        [HttpPost("{id:long}/approve/admin")]
        public IActionResult ApproveBookingByAdmin(long id)
        {
            //_bookingService.ApproveBookingByAdmin(id);
            return Ok();
        }

        // ===========================
        // Add payment to booking
        // ===========================
        [HttpPost("{id:long}/payment")]
        public IActionResult AddPayment(long id, [FromBody] Domain.Models.TourAndTravels.Booking.AddPaymentRequest request)
        {
            //_bookingService.AddPayment(id, request);
            return Ok();
        }

        // ===========================
        // Get all payments for a booking
        // ===========================
        [HttpGet("{id:long}/payments")]
        public ActionResult<List<Domain.Models.TourAndTravels.Booking.PaymentResponse>> GetPayments(long id)
        {
            //var payments = _bookingService.GetPayments(id);
            return Ok();
        }
    }
}