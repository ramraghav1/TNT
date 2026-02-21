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
            public List<CreateItineraryDayRequest> Days { get; set; } = new();
        }

        public class CreateItineraryDayRequest
        {
            public int DayNumber { get; set; }
            public string? Title { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool BreakfastIncluded { get; set; }
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public List<string> Activities { get; set; } = new();
        }

        // Response Models
        public class ItineraryResponse
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public int DurationDays { get; set; }
            public string? DifficultyLevel { get; set; }
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
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool BreakfastIncluded { get; set; }
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public List<string> Activities { get; set; } = new();
        }
        public class UpdateItineraryDayRequest
        {
            public string? Title { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }

            // Meals
            public bool? BreakfastIncluded { get; set; }
            public bool? LunchIncluded { get; set; }
            public bool? DinnerIncluded { get; set; }

            // Activities can be null if not updating
            public List<string>? Activities { get; set; }
        }
        public class UpdateItineraryRequest
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public int? DurationDays { get; set; } // Nullable so it can be updated only if needed
            public string? DifficultyLevel { get; set; }

            // Optional: allow updating days in bulk
            public List<UpdateItineraryDayRequest>? Days { get; set; }
        }
    }
}

