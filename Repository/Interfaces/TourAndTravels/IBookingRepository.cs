using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.BookingDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IBookingRepository
    {
        // Create booking → creates itinerary instance + copies days + inserts travelers
        BookingResponse CreateBooking(CreateBookingRequest request);

        // Get all bookings (lightweight list)
        List<BookingListItem> GetAllBookings();

        // Get full booking detail by instance ID
        BookingDetailResponse? GetBookingById(long instanceId);

        // Customize a single instance day (only that day is updated, others stay intact)
        BookingDayResponse? CustomizeDay(long instanceId, CustomizeDayRequest request);

        // Approve booking
        bool ApproveBooking(long instanceId, ApproveBookingRequest request);

        // Add payment
        PaymentResponse AddPayment(long instanceId, AddPaymentRequest request);

        // Get all payments for a booking
        List<PaymentResponse> GetPayments(long instanceId);

        // Update booking status
        bool UpdateStatus(long instanceId, string status);
    }
}