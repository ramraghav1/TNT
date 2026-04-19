using System.Collections.Generic;
using static Repository.DataModels.TourAndTravels.ExchangeRateDTO;

namespace Repository.Interfaces.TourAndTravels
{
    public interface IExchangeRateRepository
    {
        /// <summary>Get all current rates from exchange_rates_current.</summary>
        List<CurrentRate> GetCurrentRates();

        /// <summary>Get a specific current rate by currency code.</summary>
        CurrentRate? GetCurrentRate(string currencyCode);

        /// <summary>Upsert a current rate (insert or update).</summary>
        void UpsertCurrentRate(UpsertCurrentRate request);

        /// <summary>Record a rate snapshot in exchange_rates history.</summary>
        void InsertRateHistory(InsertRateHistory request);

        /// <summary>Get rate history, optionally filtered by currency.</summary>
        List<RateHistory> GetRateHistory(string? currency = null, int limit = 50);
    }
}
