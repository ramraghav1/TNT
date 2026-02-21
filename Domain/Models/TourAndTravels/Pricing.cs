using System;
namespace Domain.Models.TourAndTravels
{
	public class Pricing
	{
        public class BookingPricingResponse
        {
            public decimal TotalAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal BalanceAmount { get; set; }
            public List<BookingDayCostResponse> DayCosts { get; set; } = new();
        }

        public class BookingDayCostResponse
        {
            public int DayNumber { get; set; }
            public string? Location { get; set; }
            public List<BookingCostItemResponse> CostItems { get; set; } = new();
        }

        public class BookingCostItemResponse
        {
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public decimal UnitPrice { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}

