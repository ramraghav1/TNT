using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;

namespace Bussiness.MappingProfiles.TourAndTravels
{
    public class InventoryMappingProfile : Profile
    {
        public InventoryMappingProfile()
        {
            // ===========================
            // HOTELS - DTO TO DOMAIN
            // ===========================

            CreateMap<HotelDTO, Inventory.HotelResponse>()
                .ForMember(dest => dest.Rooms, opt => opt.Ignore()); // Rooms mapped separately

            CreateMap<HotelRoomDTO, Inventory.HotelRoomResponse>();

            // ===========================
            // VEHICLES - DTO TO DOMAIN
            // ===========================

            CreateMap<VehicleDTO, Inventory.VehicleResponse>();

            // ===========================
            // GUIDES - DTO TO DOMAIN
            // ===========================

            CreateMap<GuideDTO, Inventory.GuideResponse>();

            // ===========================
            // ACTIVITIES - DTO TO DOMAIN
            // ===========================

            CreateMap<ActivityDTO, Inventory.ActivityResponse>();

            // ===========================
            // AVAILABILITY - DTO TO DOMAIN
            // ===========================

            CreateMap<AvailabilityDTO, Availability.AvailabilityResponse>()
                .ForMember(dest => dest.InventoryName, opt => opt.Ignore())
                .ForMember(dest => dest.IsAvailable, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.SpecialPrice));

            CreateMap<AvailabilityBlockDTO, Availability.AvailabilityBlockResponse>()
                .ForMember(dest => dest.InventoryName, opt => opt.Ignore());

            CreateMap<PackageDepartureDTO, Availability.PackageDepartureResponse>()
                .ForMember(dest => dest.ItineraryTitle, opt => opt.Ignore())
                .ForMember(dest => dest.CanStillBook, opt => opt.Ignore());

            CreateMap<BookingInventoryDTO, Availability.BookingInventoryResponse>()
                .ForMember(dest => dest.InventoryName, opt => opt.Ignore());
        }
    }
}
