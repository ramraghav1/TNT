using System.Collections.Generic;
using AutoMapper;
using Domain.Models.TourAndTravels;
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.Pricing;

namespace Bussiness.Services.TourAndTravels
{
    public interface IPricingService
    {
        // Cost Items
        CostItemResponse CreateCostItem(CreateCostItemRequest request);
        List<CostItemResponse> GetAllCostItems();
        CostItemResponse? UpdateCostItem(long id, UpdateCostItemRequest request);
        bool DeleteCostItem(long id);

        // Cost Item Rates
        CostItemRateResponse CreateRate(CreateCostItemRateRequest request);
        List<CostItemRateResponse> GetRatesByCostItem(long costItemId);
        List<CostItemRateResponse> GetRatesByItinerary(long itineraryId);

        // Itinerary Day Costs
        DayCostResponse AssignDayCost(AssignDayCostRequest request);
        List<DayCostResponse> GetDayCosts(long itineraryDayId);
        List<DayCostResponse> GetAllDayCostsByItinerary(long itineraryId);
        bool RemoveDayCost(long id);

        // Booking Pricing
        BookingPricingResponse GetBookingPricing(long instanceId);
    }

    public class PricingService : IPricingService
    {
        private readonly IPricingRepository _repository;
        private readonly IMapper _mapper;

        public PricingService(IPricingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public CostItemResponse CreateCostItem(CreateCostItemRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.PricingDTO.CreateCostItemRequest>(request);
            var repoResponse = _repository.CreateCostItem(repoRequest);
            return _mapper.Map<CostItemResponse>(repoResponse);
        }

        public List<CostItemResponse> GetAllCostItems()
        {
            var repoResponses = _repository.GetAllCostItems();
            return _mapper.Map<List<CostItemResponse>>(repoResponses);
        }

        public CostItemResponse? UpdateCostItem(long id, UpdateCostItemRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.PricingDTO.UpdateCostItemRequest>(request);
            var repoResponse = _repository.UpdateCostItem(id, repoRequest);
            return repoResponse == null ? null : _mapper.Map<CostItemResponse>(repoResponse);
        }

        public bool DeleteCostItem(long id)
        {
            return _repository.DeleteCostItem(id);
        }

        public CostItemRateResponse CreateRate(CreateCostItemRateRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.PricingDTO.CreateCostItemRateRequest>(request);
            var repoResponse = _repository.CreateRate(repoRequest);
            return _mapper.Map<CostItemRateResponse>(repoResponse);
        }

        public List<CostItemRateResponse> GetRatesByCostItem(long costItemId)
        {
            var repoResponses = _repository.GetRatesByCostItem(costItemId);
            return _mapper.Map<List<CostItemRateResponse>>(repoResponses);
        }

        public List<CostItemRateResponse> GetRatesByItinerary(long itineraryId)
        {
            var repoResponses = _repository.GetRatesByItinerary(itineraryId);
            return _mapper.Map<List<CostItemRateResponse>>(repoResponses);
        }

        public DayCostResponse AssignDayCost(AssignDayCostRequest request)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.PricingDTO.AssignDayCostRequest>(request);
            var repoResponse = _repository.AssignDayCost(repoRequest);
            return _mapper.Map<DayCostResponse>(repoResponse);
        }

        public List<DayCostResponse> GetDayCosts(long itineraryDayId)
        {
            var repoResponses = _repository.GetDayCosts(itineraryDayId);
            return _mapper.Map<List<DayCostResponse>>(repoResponses);
        }

        public List<DayCostResponse> GetAllDayCostsByItinerary(long itineraryId)
        {
            var repoResponses = _repository.GetAllDayCostsByItinerary(itineraryId);
            return _mapper.Map<List<DayCostResponse>>(repoResponses);
        }

        public bool RemoveDayCost(long id)
        {
            return _repository.RemoveDayCost(id);
        }

        public BookingPricingResponse GetBookingPricing(long instanceId)
        {
            var repoResponse = _repository.GetBookingPricing(instanceId);
            return _mapper.Map<BookingPricingResponse>(repoResponse);
        }
    }
}
