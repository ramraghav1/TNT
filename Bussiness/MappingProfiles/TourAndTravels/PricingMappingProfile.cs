using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.DataModels.TourAndTravels;

namespace Bussiness.MappingProfiles
{
    public class PricingMappingProfile : Profile
    {
        public PricingMappingProfile()
        {
            // Cost Items
            CreateMap<PricingDTO.CreateCostItemRequest, Pricing.CreateCostItemRequest>().ReverseMap();
            CreateMap<PricingDTO.UpdateCostItemRequest, Pricing.UpdateCostItemRequest>().ReverseMap();
            CreateMap<PricingDTO.CostItemResponse, Pricing.CostItemResponse>().ReverseMap();

            // Cost Item Rates
            CreateMap<PricingDTO.CreateCostItemRateRequest, Pricing.CreateCostItemRateRequest>().ReverseMap();
            CreateMap<PricingDTO.CostItemRateResponse, Pricing.CostItemRateResponse>().ReverseMap();

            // Day Costs
            CreateMap<PricingDTO.AssignDayCostRequest, Pricing.AssignDayCostRequest>().ReverseMap();
            CreateMap<PricingDTO.DayCostResponse, Pricing.DayCostResponse>().ReverseMap();

            // Booking Pricing
            CreateMap<PricingDTO.BookingPricingResponse, Pricing.BookingPricingResponse>().ReverseMap();
            CreateMap<PricingDTO.BookingDayCostResponse, Pricing.BookingDayCostResponse>().ReverseMap();
            CreateMap<PricingDTO.BookingCostItemResponse, Pricing.BookingCostItemResponse>().ReverseMap();
        }
    }
}
