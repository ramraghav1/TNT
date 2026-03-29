using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.Pricing;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        // ===========================
        // COST ITEMS CRUD
        // ===========================

        [HttpPost("cost-items")]
        public ActionResult<CostItemResponse> CreateCostItem([FromBody] CreateCostItemRequest request)
        {
            var result = _pricingService.CreateCostItem(request);
            return Ok(result);
        }

        [HttpGet("cost-items")]
        public ActionResult<List<CostItemResponse>> GetAllCostItems()
        {
            return Ok(_pricingService.GetAllCostItems());
        }

        [HttpPut("cost-items/{id:long}")]
        public ActionResult<CostItemResponse> UpdateCostItem(long id, [FromBody] UpdateCostItemRequest request)
        {
            var result = _pricingService.UpdateCostItem(id, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("cost-items/{id:long}")]
        public IActionResult DeleteCostItem(long id)
        {
            if (!_pricingService.DeleteCostItem(id)) return NotFound();
            return Ok(new { message = "Cost item deleted." });
        }

        // ===========================
        // COST ITEM RATES
        // ===========================

        [HttpPost("rates")]
        public ActionResult<CostItemRateResponse> CreateRate([FromBody] CreateCostItemRateRequest request)
        {
            return Ok(_pricingService.CreateRate(request));
        }

        [HttpGet("rates/cost-item/{costItemId:long}")]
        public ActionResult<List<CostItemRateResponse>> GetRatesByCostItem(long costItemId)
        {
            return Ok(_pricingService.GetRatesByCostItem(costItemId));
        }

        [HttpGet("rates/itinerary/{itineraryId:long}")]
        public ActionResult<List<CostItemRateResponse>> GetRatesByItinerary(long itineraryId)
        {
            return Ok(_pricingService.GetRatesByItinerary(itineraryId));
        }

        // ===========================
        // ITINERARY DAY COSTS (Template)
        // ===========================

        [HttpPost("day-costs")]
        public ActionResult<DayCostResponse> AssignDayCost([FromBody] AssignDayCostRequest request)
        {
            return Ok(_pricingService.AssignDayCost(request));
        }

        [HttpGet("day-costs/day/{dayId:long}")]
        public ActionResult<List<DayCostResponse>> GetDayCosts(long dayId)
        {
            return Ok(_pricingService.GetDayCosts(dayId));
        }

        [HttpGet("day-costs/itinerary/{itineraryId:long}")]
        public ActionResult<List<DayCostResponse>> GetAllDayCostsByItinerary(long itineraryId)
        {
            return Ok(_pricingService.GetAllDayCostsByItinerary(itineraryId));
        }

        [HttpDelete("day-costs/{id:long}")]
        public IActionResult RemoveDayCost(long id)
        {
            if (!_pricingService.RemoveDayCost(id)) return NotFound();
            return Ok(new { message = "Day cost removed." });
        }

        // ===========================
        // BOOKING PRICING
        // ===========================

        [HttpGet("booking/{instanceId:long}")]
        public ActionResult<BookingPricingResponse> GetBookingPricing(long instanceId)
        {
            return Ok(_pricingService.GetBookingPricing(instanceId));
        }
    }
}
