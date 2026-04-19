using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Repository.Interfaces.TourAndTravels;
using static Repository.DataModels.TourAndTravels.ExchangeRateDTO;

namespace Repository.Repositories.TourAndTravels
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly IDbConnection _db;

        public ExchangeRateRepository(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>Get all current rates from exchange_rates_current.</summary>
        public List<CurrentRate> GetCurrentRates()
        {
            const string sql = @"
                SELECT currency_code, rate, last_updated, updated_by
                FROM exchange_rates_current
                ORDER BY currency_code;";

            return _db.Query<CurrentRate>(sql).AsList();
        }

        /// <summary>Get a specific current rate by currency code.</summary>
        public CurrentRate? GetCurrentRate(string currencyCode)
        {
            const string sql = @"
                SELECT currency_code, rate, last_updated, updated_by
                FROM exchange_rates_current
                WHERE currency_code = @CurrencyCode;";

            return _db.QuerySingleOrDefault<CurrentRate>(sql, new { CurrencyCode = currencyCode.ToUpperInvariant() });
        }

        /// <summary>Upsert a current rate.</summary>
        public void UpsertCurrentRate(UpsertCurrentRate request)
        {
            const string sql = @"
                INSERT INTO exchange_rates_current (currency_code, rate, last_updated, updated_by)
                VALUES (@CurrencyCode, @Rate, NOW(), @UpdatedBy)
                ON CONFLICT (currency_code) DO UPDATE SET
                    rate = EXCLUDED.rate,
                    last_updated = NOW(),
                    updated_by = EXCLUDED.updated_by;";

            _db.Execute(sql, new
            {
                CurrencyCode = request.CurrencyCode.ToUpperInvariant(),
                request.Rate,
                request.UpdatedBy
            });
        }

        /// <summary>Record a rate change in the history table.</summary>
        public void InsertRateHistory(InsertRateHistory request)
        {
            const string sql = @"
                INSERT INTO exchange_rates (base_currency, target_currency, rate, source, created_at, created_by)
                VALUES (@BaseCurrency, @TargetCurrency, @Rate, @Source, NOW(), @CreatedBy);";

            _db.Execute(sql, new
            {
                request.BaseCurrency,
                TargetCurrency = request.TargetCurrency.ToUpperInvariant(),
                request.Rate,
                request.Source,
                request.CreatedBy
            });
        }

        /// <summary>Get rate history (most recent first).</summary>
        public List<RateHistory> GetRateHistory(string? currency = null, int limit = 50)
        {
            var sql = @"
                SELECT id, base_currency, target_currency, rate, source, created_at, created_by
                FROM exchange_rates";

            if (!string.IsNullOrEmpty(currency))
                sql += " WHERE target_currency = @Currency";

            sql += " ORDER BY created_at DESC LIMIT @Limit;";

            return _db.Query<RateHistory>(sql, new
            {
                Currency = currency?.ToUpperInvariant(),
                Limit = limit
            }).AsList();
        }
    }
}
