using System;
namespace Domain.Models.TourAndTravels
{
	public class Itinerary
	{
        public class CreateItineraryRequest
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int DurationDays { get; set; }
            public string? DifficultyLevel { get; set; }
            public string PricingMode { get; set; } = "DAILY_ACTIVITY";
            public decimal? OverallPrice { get; set; }
            public List<CreateItineraryDayRequest> Days { get; set; } = new();
        }

        public class CreateItineraryDayRequest
        {
            public int DayNumber { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool BreakfastIncluded { get; set; }
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public decimal? DailyCost { get; set; }
            public long? HotelId { get; set; }
            public long? GuideId { get; set; }
            public List<string> Activities { get; set; } = new();
            public List<DayCostInput> Costs { get; set; } = new();
        }

        public class DayCostInput
        {
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }

        // Response Models
        public class ItineraryResponse
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int DurationDays { get; set; }
            public string? DifficultyLevel { get; set; }
            public string PricingMode { get; set; } = "DAILY_ACTIVITY";
            public decimal? OverallPrice { get; set; }
        }

        public class ItineraryDetailResponse : ItineraryResponse
        {
            public List<ItineraryDayResponse> Days { get; set; } = new();
        }

        public class ItineraryDayResponse
        {
            public long Id { get; set; }
            public int DayNumber { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool BreakfastIncluded { get; set; }
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public decimal? DailyCost { get; set; }
            public long? HotelId { get; set; }
            public long? GuideId { get; set; }
            public List<string> Activities { get; set; } = new();
            public List<DayCostInput> Costs { get; set; } = new();
        }
        public class UpdateItineraryDayRequest
        {
            public int DayNumber { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool BreakfastIncluded { get; set; }
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public decimal? DailyCost { get; set; }
            public long? HotelId { get; set; }
            public long? GuideId { get; set; }
            public List<string> Activities { get; set; } = new();
            public List<DayCostInput> Costs { get; set; } = new();
        }
        public class UpdateItineraryRequest
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int DurationDays { get; set; }
            public string? DifficultyLevel { get; set; }
            public string PricingMode { get; set; } = "DAILY_ACTIVITY";
            public decimal? OverallPrice { get; set; }
            public List<UpdateItineraryDayRequest> Days { get; set; } = new();
        }
    }
}

