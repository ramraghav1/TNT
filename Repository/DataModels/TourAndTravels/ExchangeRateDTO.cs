namespace Repository.DataModels.TourAndTravels
{
    public class ExchangeRateDTO
    {
        // ===========================
        // Current rate record from exchange_rates_current table
        // ===========================
        public class CurrentRate
        {
            public string CurrencyCode { get; set; } = string.Empty;
            public decimal Rate { get; set; }
            public DateTime LastUpdated { get; set; }
            public string? UpdatedBy { get; set; }
        }

        // ===========================
        // Rate history from exchange_rates table
        // ===========================
        public class RateHistory
        {
            public long Id { get; set; }
            public string BaseCurrency { get; set; } = "NPR";
            public string TargetCurrency { get; set; } = string.Empty;
            public decimal Rate { get; set; }
            public string? Source { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? CreatedBy { get; set; }
        }

        // ===========================
        // Upsert current rate request
        // ===========================
        public class UpsertCurrentRate
        {
            public string CurrencyCode { get; set; } = string.Empty;
            public decimal Rate { get; set; }
            public string? UpdatedBy { get; set; }
        }

        // ===========================
        // Insert rate history record
        // ===========================
        public class InsertRateHistory
        {
            public string BaseCurrency { get; set; } = "NPR";
            public string TargetCurrency { get; set; } = string.Empty;
            public decimal Rate { get; set; }
            public string Source { get; set; } = "manual";
            public string? CreatedBy { get; set; }
        }
    }
}
