using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;

namespace Bussiness.MappingProfiles
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            // Request: Domain to DTO
            CreateMap<Booking.CreateBookingRequest, BookingDTO.CreateBookingRequest>().ReverseMap();
            CreateMap<Booking.TravelerRequest, BookingDTO.TravelerRequest>().ReverseMap();
            CreateMap<Booking.CustomizeDayRequest, BookingDTO.CustomizeDayRequest>().ReverseMap();
            CreateMap<Booking.AddPaymentRequest, BookingDTO.AddPaymentRequest>().ReverseMap();
            CreateMap<Booking.ApproveBookingRequest, BookingDTO.ApproveBookingRequest>().ReverseMap();

            // Response: DTO to Domain
            CreateMap<BookingDTO.BookingResponse, Booking.BookingResponse>().ReverseMap();
            CreateMap<BookingDTO.BookingDetailResponse, Booking.BookingDetailResponse>().ReverseMap();
            CreateMap<BookingDTO.BookingDayResponse, Booking.BookingDayResponse>().ReverseMap();
            CreateMap<BookingDTO.TravelerResponse, Booking.TravelerResponse>().ReverseMap();
            CreateMap<BookingDTO.PaymentResponse, Booking.PaymentResponse>().ReverseMap();
            CreateMap<BookingDTO.BookingListItem, Booking.BookingListItem>().ReverseMap();

            // Dashboard stats
            CreateMap<BookingDTO.DashboardStatsDTO, Booking.DashboardStats>().ReverseMap();
            CreateMap<BookingDTO.MonthlyBookingCountDTO, Booking.MonthlyBookingCount>().ReverseMap();
            CreateMap<BookingDTO.RevenueByMonthDTO, Booking.RevenueByMonth>().ReverseMap();
            CreateMap<BookingDTO.TopItineraryDTO, Booking.TopItinerary>().ReverseMap();
            CreateMap<BookingDTO.RecentBookingDTO, Booking.RecentBooking>().ReverseMap();
            CreateMap<BookingDTO.StatusCountDTO, Booking.StatusCount>().ReverseMap();
        }
    }
}
