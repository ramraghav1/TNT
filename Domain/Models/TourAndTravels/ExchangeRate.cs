namespace Domain.Models.TourAndTravels
{
    /// <summary>
    /// Multi-currency domain models.
    /// Base currency: NPR. Supported: NPR, INR, USD.
    /// All prices stored in NPR; frontend converts for display only.
    /// </summary>
    public class ExchangeRate
    {
        // ===========================
        // Current rates response (GET /api/exchange-rates)
        // ===========================
        public class ExchangeRatesResponse
        {
            public string Base { get; set; } = "NPR";
            public Dictionary<string, decimal> Rates { get; set; } = new();
            public DateTime LastUpdated { get; set; }
        }

        // ===========================
        // Update rate request (admin)
        // ===========================
        public class UpdateRateRequest
        {
            public string Currency { get; set; } = string.Empty;  // "INR" or "USD"
            public decimal Rate { get; set; }
        }

        // ===========================
        // Bulk update rates (admin)
        // ===========================
        public class BulkUpdateRatesRequest
        {
            public Dictionary<string, decimal> Rates { get; set; } = new();
        }

        // ===========================
        // Rate history item
        // ===========================
        public class RateHistoryItem
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
        // Booking Preview Request
        // ===========================
        public class BookingPreviewRequest
        {
            public long ItineraryId { get; set; }
            public string Currency { get; set; } = "NPR";
            public int TravelerCount { get; set; } = 1;
        }

        // ===========================
        // Booking Preview Response
        // ===========================
        public class BookingPreviewResponse
        {
            public long ItineraryId { get; set; }
            public string Currency { get; set; } = "NPR";
            public decimal ExchangeRate { get; set; } = 1.0m;
            public decimal BasePriceNpr { get; set; }
            public decimal FinalPrice { get; set; }
            public DateTime RateTimestamp { get; set; }
            public string Note { get; set; } = string.Empty;
        }

        // ===========================
        // Rounding rules per currency
        // ===========================
        public static decimal RoundForCurrency(decimal amount, string currency)
        {
            return currency.ToUpperInvariant() switch
            {
                "NPR" => Math.Round(amount, 0, MidpointRounding.AwayFromZero),
                "INR" => Math.Round(amount, 0, MidpointRounding.AwayFromZero),
                "USD" => Math.Round(amount, 2, MidpointRounding.AwayFromZero),
                _ => Math.Round(amount, 2, MidpointRounding.AwayFromZero)
            };
        }

        // ===========================
        // Supported currencies
        // ===========================
        public static readonly string[] SupportedCurrencies = { "NPR", "INR", "USD" };

        public static readonly Dictionary<string, string> CurrencySymbols = new()
        {
            { "NPR", "Rs" },
            { "INR", "₹" },
            { "USD", "$" }
        };
    }
}
