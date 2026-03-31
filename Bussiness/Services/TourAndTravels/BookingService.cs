using System;
using System.Collections.Generic;
using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.Booking;

namespace Bussiness.Services.TourAndTravels
{
    public interface IBookingService
    {
        BookingResponse CreateBooking(CreateBookingRequest request);
        List<BookingListItem> GetAllBookings();
        BookingDetailResponse? GetBookingById(long id);
        BookingDayResponse? CustomizeDay(long instanceId, CustomizeDayRequest request);
        bool ApproveBooking(long id, ApproveBookingRequest request);
        PaymentResponse AddPayment(long id, AddPaymentRequest request);
        List<PaymentResponse> GetPayments(long id);
        bool UpdateStatus(long id, string status);
        DashboardStats GetDashboardStats();
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public BookingService(IBookingRepository repository, IMapper mapper, INotificationService notificationService)
        {
            _repository = repository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        // ===========================
        // Create a new booking (from template or reuse existing instance)
        // ===========================
        public BookingResponse CreateBooking(CreateBookingRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.CreateBookingRequest>(request);
            var repoResponse = _repository.CreateBooking(repoRequest);
            var result = _mapper.Map<BookingResponse>(repoResponse);

            // Push real-time notification
            _ = _notificationService.CreateAndBroadcastAsync(new Domain.Models.CreateNotification
            {
                Type = "booking",
                Title = "New Booking",
                Message = $"New booking created (Ref: {result.BookingReference})",
                Link = "/booking-list",
                Icon = "pi-bookmark"
            });

            return result;
        }

        // ===========================
        // Get all bookings (lightweight list)
        // ===========================
        public List<BookingListItem> GetAllBookings()
        {
            var repoResponses = _repository.GetAllBookings();
            return _mapper.Map<List<BookingListItem>>(repoResponses);
        }

        // ===========================
        // Get booking by id (full detail)
        // ===========================
        public BookingDetailResponse? GetBookingById(long id)
        {
            var repoResponse = _repository.GetBookingById(id);
            return repoResponse == null ? null : _mapper.Map<BookingDetailResponse>(repoResponse);
        }

        // ===========================
        // Customize a single instance day
        // Only updates that day — all other itinerary days stay intact
        // ===========================
        public BookingDayResponse? CustomizeDay(long instanceId, CustomizeDayRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.CustomizeDayRequest>(request);
            var repoResponse = _repository.CustomizeDay(instanceId, repoRequest);
            return repoResponse == null ? null : _mapper.Map<BookingDayResponse>(repoResponse);
        }

        // ===========================
        // Approve booking (traveler or admin)
        // ===========================
        public bool ApproveBooking(long id, ApproveBookingRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.ApproveBookingRequest>(request);
            return _repository.ApproveBooking(id, repoRequest);
        }

        // ===========================
        // Add payment
        // ===========================
        public PaymentResponse AddPayment(long id, AddPaymentRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.AddPaymentRequest>(request);
            var repoResponse = _repository.AddPayment(id, repoRequest);
            return _mapper.Map<PaymentResponse>(repoResponse);
        }

        // ===========================
        // Get all payments for a booking
        // ===========================
        public List<PaymentResponse> GetPayments(long id)
        {
            var repoResponses = _repository.GetPayments(id);
            return _mapper.Map<List<PaymentResponse>>(repoResponses);
        }

        // ===========================
        // Update booking status
        // ===========================
        public bool UpdateStatus(long id, string status)
        {
            return _repository.UpdateStatus(id, status);
        }

        // ===========================
        // Dashboard Stats
        // ===========================
        public DashboardStats GetDashboardStats()
        {
            var repoStats = _repository.GetDashboardStats();
            return _mapper.Map<DashboardStats>(repoStats);
        }
    }
}