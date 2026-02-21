using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;

namespace Bussiness.MappingProfiles
{
    public class ItineraryMappingProfile : Profile
    {
        public ItineraryMappingProfile()
        {
            // ===========================
            // CREATE MAPPINGS (Repository DTO → Domain)
            // ===========================
            CreateMap<ItineraryDTO.CreateItineraryRequest, Itinerary.CreateItineraryRequest>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.DurationDays))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel))
                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.Days));

            CreateMap<ItineraryDTO.CreateItineraryDayRequest, Itinerary.CreateItineraryDayRequest>()
                .ForMember(dest => dest.DayNumber, opt => opt.MapFrom(src => src.DayNumber))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Accommodation, opt => opt.MapFrom(src => src.Accommodation))
                .ForMember(dest => dest.Transport, opt => opt.MapFrom(src => src.Transport))
                .ForMember(dest => dest.BreakfastIncluded, opt => opt.MapFrom(src => src.BreakfastIncluded))
                .ForMember(dest => dest.LunchIncluded, opt => opt.MapFrom(src => src.LunchIncluded))
                .ForMember(dest => dest.DinnerIncluded, opt => opt.MapFrom(src => src.DinnerIncluded))
                .ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.Activities));

            // ===========================
            // UPDATE MAPPINGS
            // ===========================
            CreateMap<ItineraryDTO.UpdateItineraryRequest, Itinerary.UpdateItineraryRequest>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.DurationDays))
                .ForMember(dest => dest.DifficultyLevel, opt => opt.MapFrom(src => src.DifficultyLevel))
                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.Days));

            CreateMap<ItineraryDTO.UpdateItineraryDayRequest, Itinerary.UpdateItineraryDayRequest>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Accommodation, opt => opt.MapFrom(src => src.Accommodation))
                .ForMember(dest => dest.Transport, opt => opt.MapFrom(src => src.Transport))
                .ForMember(dest => dest.BreakfastIncluded, opt => opt.MapFrom(src => src.BreakfastIncluded))
                .ForMember(dest => dest.LunchIncluded, opt => opt.MapFrom(src => src.LunchIncluded))
                .ForMember(dest => dest.DinnerIncluded, opt => opt.MapFrom(src => src.DinnerIncluded))
                .ForMember(dest => dest.Activities, opt => opt.MapFrom(src => src.Activities));

            // ===========================
            // RESPONSE MAPPINGS (Repository DTO → Domain Response)
            // ===========================
            CreateMap<ItineraryDTO.ItineraryResponse, Itinerary.ItineraryResponse>().ReverseMap();
            CreateMap<ItineraryDTO.ItineraryDetailResponse, Itinerary.ItineraryDetailResponse>()
                .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.Days))
                .ReverseMap();
            CreateMap<ItineraryDTO.ItineraryDayResponse, Itinerary.ItineraryDayResponse>().ReverseMap();
        }
    }
}