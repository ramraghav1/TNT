using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;

namespace Bussiness.MappingProfiles
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            // ===========================
            // CREATE BOOKING
            // ===========================
            CreateMap<BookingDTO.CreateBookingRequest, Booking.CreateBookingRequest>()
                .ForMember(dest => dest.ItineraryId, opt => opt.MapFrom(src => src.ItineraryId))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Travelers, opt => opt.MapFrom(src => src.Travelers))
                .ForMember(dest => dest.SpecialRequests, opt => opt.MapFrom(src => src.SpecialRequests));

            CreateMap<BookingDTO.TravelerRequest, Booking.TravelerRequest>().ReverseMap();

            // ===========================
            // UPDATE / CUSTOMIZE BOOKING
            // ===========================
            CreateMap<BookingDTO.UpdateBookingRequest, Booking.UpdateBookingRequest>()
                .ForMember(dest => dest.TravelerName, opt => opt.MapFrom(src => src.TravelerName))
                .ForMember(dest => dest.TravelerEmail, opt => opt.MapFrom(src => src.TravelerEmail))
                .ForMember(dest => dest.TravelerPhone, opt => opt.MapFrom(src => src.TravelerPhone))
                .ForMember(dest => dest.Adults, opt => opt.MapFrom(src => src.Adults))
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children))
                .ForMember(dest => dest.Seniors, opt => opt.MapFrom(src => src.Seniors))
                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.Days))
                .ForMember(dest => dest.SpecialRequests, opt => opt.MapFrom(src => src.SpecialRequests));

            CreateMap<BookingDTO.UpdateBookingDayRequest, Booking.UpdateBookingDayRequest>().ReverseMap();

            // ===========================
            // CUSTOMIZE BOOKING DAYS
            // ===========================
            CreateMap<BookingDTO.CustomizeDayRequest, Booking.CustomizeDayRequest>().ReverseMap();

            // ===========================
            // RESPONSES
            // ===========================
            CreateMap<BookingDTO.BookingResponse, Booking.BookingResponse>().ReverseMap();
            CreateMap<BookingDTO.BookingDetailResponse, Booking.BookingDetailResponse>().ReverseMap();
            CreateMap<BookingDTO.PaymentResponse, Booking.PaymentResponse>().ReverseMap();

            // Optional: Add map for AddPaymentRequest
            CreateMap<BookingDTO.AddPaymentRequest, Booking.AddPaymentRequest>().ReverseMap();
        }
    }
}