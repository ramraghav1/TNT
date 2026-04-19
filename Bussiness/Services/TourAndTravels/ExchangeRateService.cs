using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.ExchangeRate;
using static Repository.DataModels.TourAndTravels.ExchangeRateDTO;

namespace Bussiness.Services.TourAndTravels
{
    public interface IExchangeRateService
    {
        /// <summary>Get current exchange rates (cached).</summary>
        ExchangeRatesResponse GetCurrentRates();

        /// <summary>Get rate for a specific currency.</summary>
        decimal GetRate(string currency);

        /// <summary>Convert amount from NPR to target currency.</summary>
        decimal ConvertFromNpr(decimal amountNpr, string targetCurrency);

        /// <summary>Convert amount from target currency to NPR.</summary>
        decimal ConvertToNpr(decimal amount, string fromCurrency);

        /// <summary>Update a single rate (admin). Returns updated rates.</summary>
        ExchangeRatesResponse UpdateRate(string currency, decimal rate, string? updatedBy);

        /// <summary>Bulk update rates (admin). Returns updated rates.</summary>
        ExchangeRatesResponse BulkUpdateRates(Dictionary<string, decimal> rates, string? updatedBy);

        /// <summary>Get rate change history.</summary>
        List<RateHistoryItem> GetRateHistory(string? currency, int limit);

        /// <summary>Preview booking price in a target currency.</summary>
        BookingPreviewResponse PreviewBookingPrice(long itineraryId, string currency, int travelerCount);

        /// <summary>Invalidate rate cache (after update).</summary>
        void InvalidateCache();
    }

    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateRepository _repository;
        private readonly IItineraryRepository _itineraryRepository;
        private readonly IMemoryCache _cache;
        private readonly IHubContext<NotificationHub> _hubContext;
        private const string CacheKey = "exchange_rates_current";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public ExchangeRateService(
            IExchangeRateRepository repository,
            IItineraryRepository itineraryRepository,
            IMemoryCache cache,
            IHubContext<NotificationHub> hubContext)
        {
            _repository = repository;
            _itineraryRepository = itineraryRepository;
            _cache = cache;
            _hubContext = hubContext;
        }

        public ExchangeRatesResponse GetCurrentRates()
        {
            if (_cache.TryGetValue(CacheKey, out ExchangeRatesResponse? cached) && cached != null)
                return cached;

            var dbRates = _repository.GetCurrentRates();
            var response = new ExchangeRatesResponse
            {
                Base = "NPR",
                Rates = dbRates.ToDictionary(r => r.CurrencyCode, r => r.Rate),
                LastUpdated = dbRates.Max(r => r.LastUpdated)
            };

            // Ensure NPR = 1.0 always
            response.Rates["NPR"] = 1.0m;

            _cache.Set(CacheKey, response, CacheDuration);
            return response;
        }

        public decimal GetRate(string currency)
        {
            if (string.IsNullOrEmpty(currency) || currency.Equals("NPR", StringComparison.OrdinalIgnoreCase))
                return 1.0m;

            var rates = GetCurrentRates();
            if (rates.Rates.TryGetValue(currency.ToUpperInvariant(), out var rate))
                return rate;

            throw new ArgumentException($"Unsupported currency: {currency}");
        }

        public decimal ConvertFromNpr(decimal amountNpr, string targetCurrency)
        {
            if (targetCurrency.Equals("NPR", StringComparison.OrdinalIgnoreCase))
                return RoundForCurrency(amountNpr, "NPR");

            var rate = GetRate(targetCurrency);
            return RoundForCurrency(amountNpr * rate, targetCurrency);
        }

        public decimal ConvertToNpr(decimal amount, string fromCurrency)
        {
            if (fromCurrency.Equals("NPR", StringComparison.OrdinalIgnoreCase))
                return RoundForCurrency(amount, "NPR");

            var rate = GetRate(fromCurrency);
            if (rate == 0) throw new InvalidOperationException($"Rate for {fromCurrency} is zero.");
            return RoundForCurrency(amount / rate, "NPR");
        }

        public ExchangeRatesResponse UpdateRate(string currency, decimal rate, string? updatedBy)
        {
            currency = currency.ToUpperInvariant();
            if (currency == "NPR")
                throw new ArgumentException("Cannot change the base currency rate (NPR is always 1.0).");

            if (!SupportedCurrencies.Contains(currency))
                throw new ArgumentException($"Unsupported currency: {currency}");

            if (rate <= 0)
                throw new ArgumentException("Rate must be positive.");

            // Update current
            _repository.UpsertCurrentRate(new UpsertCurrentRate
            {
                CurrencyCode = currency,
                Rate = rate,
                UpdatedBy = updatedBy
            });

            // Record history
            _repository.InsertRateHistory(new InsertRateHistory
            {
                BaseCurrency = "NPR",
                TargetCurrency = currency,
                Rate = rate,
                Source = "manual",
                CreatedBy = updatedBy
            });

            InvalidateCache();
            var updatedRates = GetCurrentRates();

            // Broadcast via SignalR
            BroadcastRateUpdate(updatedRates);

            return updatedRates;
        }

        public ExchangeRatesResponse BulkUpdateRates(Dictionary<string, decimal> rates, string? updatedBy)
        {
            foreach (var kvp in rates)
            {
                var currency = kvp.Key.ToUpperInvariant();
                if (currency == "NPR") continue; // Skip base currency

                if (!SupportedCurrencies.Contains(currency))
                    throw new ArgumentException($"Unsupported currency: {currency}");

                if (kvp.Value <= 0)
                    throw new ArgumentException($"Rate for {currency} must be positive.");

                _repository.UpsertCurrentRate(new UpsertCurrentRate
                {
                    CurrencyCode = currency,
                    Rate = kvp.Value,
                    UpdatedBy = updatedBy
                });

                _repository.InsertRateHistory(new InsertRateHistory
                {
                    BaseCurrency = "NPR",
                    TargetCurrency = currency,
                    Rate = kvp.Value,
                    Source = "manual_bulk",
                    CreatedBy = updatedBy
                });
            }

            InvalidateCache();
            var updatedRates = GetCurrentRates();
            BroadcastRateUpdate(updatedRates);
            return updatedRates;
        }

        public List<RateHistoryItem> GetRateHistory(string? currency, int limit)
        {
            var history = _repository.GetRateHistory(currency, limit);
            return history.Select(h => new RateHistoryItem
            {
                Id = h.Id,
                BaseCurrency = h.BaseCurrency,
                TargetCurrency = h.TargetCurrency,
                Rate = h.Rate,
                Source = h.Source,
                CreatedAt = h.CreatedAt,
                CreatedBy = h.CreatedBy
            }).ToList();
        }

        public BookingPreviewResponse PreviewBookingPrice(long itineraryId, string currency, int travelerCount)
        {
            currency = currency.ToUpperInvariant();
            if (!SupportedCurrencies.Contains(currency))
                throw new ArgumentException($"Unsupported currency: {currency}");

            // Get itinerary base price in NPR
            var itinerary = _itineraryRepository.GetItineraryById(itineraryId);
            if (itinerary == null)
                throw new KeyNotFoundException($"Itinerary {itineraryId} not found.");

            // overall_price is stored in NPR (base currency)
            decimal basePriceNpr = (itinerary.OverallPrice ?? 0m) * Math.Max(travelerCount, 1);

            var rate = GetRate(currency);
            var finalPrice = RoundForCurrency(basePriceNpr * rate, currency);
            var ratesResponse = GetCurrentRates();

            return new BookingPreviewResponse
            {
                ItineraryId = itineraryId,
                Currency = currency,
                ExchangeRate = rate,
                BasePriceNpr = basePriceNpr,
                FinalPrice = finalPrice,
                RateTimestamp = ratesResponse.LastUpdated,
                Note = currency == "NPR"
                    ? "Price in base currency (NPR)."
                    : "Final price will be confirmed at checkout using the latest exchange rate."
            };
        }

        public void InvalidateCache()
        {
            _cache.Remove(CacheKey);
        }

        private void BroadcastRateUpdate(ExchangeRatesResponse rates)
        {
            try
            {
                _hubContext.Clients.All.SendAsync("RateUpdated", new
                {
                    rates = rates.Rates,
                    lastUpdated = rates.LastUpdated
                });
            }
            catch
            {
                // SignalR broadcast failure should not break the rate update
            }
        }
    }
}
