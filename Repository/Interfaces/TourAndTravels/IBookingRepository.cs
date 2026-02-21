using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.BookingDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IBookingRepository
    {
        // Booking CRUD
        BookingResponse CreateBooking(CreateBookingRequest request);
        List<BookingResponse> GetAllBookings();
        BookingDetailResponse? GetBookingById(long id);
        BookingResponse? UpdateBooking(long id, UpdateBookingRequest request);
        bool DeleteBooking(long id);

        // Booking Day operations
        BookingDayResponse? AddDayToBooking(long bookingId, CustomizeDayRequest request);
        BookingDayResponse? UpdateBookingDay(long bookingId, long dayId, UpdateBookingDayRequest request);

        // Payment operations
        bool AddPayment(long bookingId, decimal amount, string paymentMethod);
        List<BookingPaymentDTO> GetPaymentsForBooking(long bookingId);
    }

    public class BookingPaymentDTO
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }
}