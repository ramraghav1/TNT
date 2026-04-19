using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.ExchangeRate;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/exchange-rates")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IExchangeRateService _service;

        public ExchangeRatesController(IExchangeRateService service)
        {
            _service = service;
        }

        // ─────────────────────────────────────────────
        // GET /api/exchange-rates
        // Public — used by frontend on app load
        // Returns: { base: "NPR", rates: { INR: 0.625, USD: 0.0075 }, lastUpdated: ... }
        // ─────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<ExchangeRatesResponse> GetRates()
        {
            return Ok(_service.GetCurrentRates());
        }

        // ─────────────────────────────────────────────
        // PUT /api/exchange-rates
        // Admin — update a single currency rate
        // Body: { currency: "USD", rate: 0.0076 }
        // ─────────────────────────────────────────────
        [HttpPut]
        [Authorize]
        public ActionResult<ExchangeRatesResponse> UpdateRate([FromBody] UpdateRateRequest request)
        {
            try
            {
                var updatedBy = User.Identity?.Name ?? "admin";
                var result = _service.UpdateRate(request.Currency, request.Rate, updatedBy);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ─────────────────────────────────────────────
        // PUT /api/exchange-rates/bulk
        // Admin — update multiple rates at once
        // Body: { rates: { "INR": 0.625, "USD": 0.0076 } }
        // ─────────────────────────────────────────────
        [HttpPut("bulk")]
        [Authorize]
        public ActionResult<ExchangeRatesResponse> BulkUpdateRates([FromBody] BulkUpdateRatesRequest request)
        {
            try
            {
                var updatedBy = User.Identity?.Name ?? "admin";
                var result = _service.BulkUpdateRates(request.Rates, updatedBy);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ─────────────────────────────────────────────
        // GET /api/exchange-rates/history
        // Admin — view rate change history
        // ─────────────────────────────────────────────
        [HttpGet("history")]
        [Authorize]
        public ActionResult<List<RateHistoryItem>> GetHistory(
            [FromQuery] string? currency = null,
            [FromQuery] int limit = 50)
        {
            return Ok(_service.GetRateHistory(currency, limit));
        }

        // ─────────────────────────────────────────────
        // POST /api/exchange-rates/booking-preview
        // Public or Auth — preview a booking price in selected currency
        // Body: { itineraryId, currency, travelerCount }
        // ─────────────────────────────────────────────
        [HttpPost("booking-preview")]
        [AllowAnonymous]
        public ActionResult<BookingPreviewResponse> PreviewBooking([FromBody] BookingPreviewRequest request)
        {
            try
            {
                var result = _service.PreviewBookingPrice(
                    request.ItineraryId,
                    request.Currency,
                    request.TravelerCount);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ─────────────────────────────────────────────
        // GET /api/exchange-rates/supported
        // Public — list of supported currencies with symbols
        // ─────────────────────────────────────────────
        [HttpGet("supported")]
        [AllowAnonymous]
        public IActionResult GetSupportedCurrencies()
        {
            var currencies = new List<object>();
            foreach (var code in SupportedCurrencies)
            {
                currencies.Add(new
                {
                    code,
                    symbol = CurrencySymbols.GetValueOrDefault(code, code),
                    isBase = code == "NPR"
                });
            }
            return Ok(currencies);
        }
    }
}
