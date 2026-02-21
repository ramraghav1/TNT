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
        List<BookingResponse> GetAllBookings();
        BookingDetailResponse? GetBookingById(long id);
        BookingResponse? UpdateBooking(long id, UpdateBookingRequest request);
        void ApproveBookingByTraveler(long id);
        void ApproveBookingByAdmin(long id);
        void AddPayment(long id, AddPaymentRequest request);
        List<PaymentResponse> GetPayments(long id);
    }

    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // ===========================
        // Create a new booking
        // ===========================
        public BookingResponse CreateBooking(CreateBookingRequest request)
        {
            // Map domain request → repository DTO
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.CreateBookingRequest>(request);

            // Call repository
            var repoResponse = _repository.CreateBooking(repoRequest);

            // Map repository response → domain response
            return _mapper.Map<BookingResponse>(repoResponse);
        }

        // ===========================
        // Get all bookings
        // ===========================
        public List<BookingResponse> GetAllBookings()
        {
            var repoResponses = _repository.GetAllBookings();
            return _mapper.Map<List<BookingResponse>>(repoResponses);
        }

        // ===========================
        // Get booking by id
        // ===========================
        public BookingDetailResponse? GetBookingById(long id)
        {
            var repoResponse = _repository.GetBookingById(id);
            return repoResponse == null ? null : _mapper.Map<BookingDetailResponse>(repoResponse);
        }

        // ===========================
        // Update / customize booking
        // ===========================
        public BookingResponse? UpdateBooking(long id, UpdateBookingRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.UpdateBookingRequest>(request);
            var repoResponse = _repository.UpdateBooking(id, repoRequest);
            return repoResponse == null ? null : _mapper.Map<BookingResponse>(repoResponse);
        }

        // ===========================
        // Approvals
        // ===========================
        public void ApproveBookingByTraveler(long id)
        {
            _repository.ApproveBookingByTraveler(id);
        }

        public void ApproveBookingByAdmin(long id)
        {
            _repository.ApproveBookingByAdmin(id);
        }

        // ===========================
        // Payments
        // ===========================
        public void AddPayment(long id, AddPaymentRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.BookingDTO.AddPaymentRequest>(request);
            _repository.AddPayment(id, repoRequest);
        }

        public List<PaymentResponse> GetPayments(long id)
        {
            var repoResponses = _repository.GetPayments(id);
            return _mapper.Map<List<PaymentResponse>>(repoResponses);
        }
    }
}