using System;

namespace Repository.DataModels.Remittance
{
    public class CountryDTO
    {
        public class CreateCountryRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Iso2Code { get; set; } = string.Empty;
            public string Iso3Code { get; set; } = string.Empty;
            public string? PhoneCode { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
            public string? CurrencyName { get; set; }
        }

        public class UpdateCountryRequest
        {
            public string? Name { get; set; }
            public string? PhoneCode { get; set; }
            public string? CurrencyCode { get; set; }
            public string? CurrencyName { get; set; }
            public bool? IsActive { get; set; }
        }

        public class CountryResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Iso2Code { get; set; } = string.Empty;
            public string Iso3Code { get; set; } = string.Empty;
            public string? PhoneCode { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
            public string? CurrencyName { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
