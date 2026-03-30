using System;
namespace Domain.Models.TourAndTravels
{
	public class Pricing
	{
        // ===========================
        // COST ITEM CRUD
        // ===========================
        public class CreateCostItemRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string UnitType { get; set; } = string.Empty;
        }

        public class UpdateCostItemRequest
        {
            public string? Name { get; set; }
            public string? Category { get; set; }
            public string? UnitType { get; set; }
            public bool? IsActive { get; set; }
        }

        public class CostItemResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string UnitType { get; set; } = string.Empty;
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // COST ITEM RATES
        // ===========================
        public class CreateCostItemRateRequest
        {
            public long CostItemId { get; set; }
            public string? Location { get; set; }
            public long? ItineraryId { get; set; }
            public decimal Price { get; set; }
            public string Currency { get; set; } = "USD";
            public DateTime? EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
        }

        public class CostItemRateResponse
        {
            public long Id { get; set; }
            public long CostItemId { get; set; }
            public string CostItemName { get; set; } = string.Empty;
            public string? Location { get; set; }
            public long? ItineraryId { get; set; }
            public decimal Price { get; set; }
            public string Currency { get; set; } = "USD";
            public DateTime? EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
        }

        // ===========================
        // ITINERARY DAY COST MAPPING
        // ===========================
        public class AssignDayCostRequest
        {
            public long ItineraryDayId { get; set; }
            public long CostItemId { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public class DayCostResponse
        {
            public long Id { get; set; }
            public long ItineraryDayId { get; set; }
            public long CostItemId { get; set; }
            public string CostItemName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string UnitType { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }

        // ===========================
        // BOOKING PRICING RESPONSES
        // ===========================
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

