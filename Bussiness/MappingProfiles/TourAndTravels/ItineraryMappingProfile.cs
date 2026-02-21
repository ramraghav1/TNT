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
            // CREATE MAPPINGS (BOTH DIRECTIONS)
            // ===========================

            CreateMap<
                ItineraryDTO.CreateItineraryRequest,
                Itinerary.CreateItineraryRequest
            >().ReverseMap();

            CreateMap<
                ItineraryDTO.CreateItineraryDayRequest,
                Itinerary.CreateItineraryDayRequest
            >().ReverseMap();

            // ===========================
            // UPDATE MAPPINGS (BOTH DIRECTIONS)
            // ===========================

            CreateMap<
                ItineraryDTO.UpdateItineraryRequest,
                Itinerary.UpdateItineraryRequest
            >().ReverseMap();

            CreateMap<
                ItineraryDTO.UpdateItineraryDayRequest,
                Itinerary.UpdateItineraryDayRequest
            >().ReverseMap();

            // ===========================
            // RESPONSE MAPPINGS (BOTH DIRECTIONS)
            // ===========================

            CreateMap<
                ItineraryDTO.ItineraryResponse,
                Itinerary.ItineraryResponse
            >().ReverseMap();

            CreateMap<
                ItineraryDTO.ItineraryDetailResponse,
                Itinerary.ItineraryDetailResponse
            >().ReverseMap();

            CreateMap<
                ItineraryDTO.ItineraryDayResponse,
                Itinerary.ItineraryDayResponse
            >().ReverseMap();
        }
    }
}