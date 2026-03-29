using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.PricingDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IPricingRepository
    {
        // Cost Items CRUD
        CostItemResponse CreateCostItem(CreateCostItemRequest request);
        List<CostItemResponse> GetAllCostItems();
        CostItemResponse? UpdateCostItem(long id, UpdateCostItemRequest request);
        bool DeleteCostItem(long id);

        // Cost Item Rates
        CostItemRateResponse CreateRate(CreateCostItemRateRequest request);
        List<CostItemRateResponse> GetRatesByCostItem(long costItemId);
        List<CostItemRateResponse> GetRatesByItinerary(long itineraryId);

        // Itinerary Day Costs (template)
        DayCostResponse AssignDayCost(AssignDayCostRequest request);
        List<DayCostResponse> GetDayCosts(long itineraryDayId);
        List<DayCostResponse> GetAllDayCostsByItinerary(long itineraryId);
        bool RemoveDayCost(long id);

        // Booking Pricing
        BookingPricingResponse GetBookingPricing(long instanceId);
    }
}
