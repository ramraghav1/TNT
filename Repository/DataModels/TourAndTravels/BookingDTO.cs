using System;
using static Repository.DataModels.TourAndTravels.ItineraryDTO;

namespace Repository.DataModels.TourAndTravels
{
	public class BookingDTO
	{
        public class CreateBookingRequest
        {
            public long ItineraryId { get; set; } // Reference to the itinerary template
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<TravelerRequest> Travelers { get; set; } = new();
            public string? SpecialRequests { get; set; } // Optional notes
        }

        // Traveler information
        public class TravelerRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string? ContactNumber { get; set; }
            public string? Email { get; set; }
            public string? Nationality { get; set; }
            public int Adults { get; set; } = 1;
            public int Children { get; set; } = 0;
            public int Seniors { get; set; } = 0;
        }

        // ===========================
        // Request to customize booking
        // ===========================
        public class CustomizeBookingRequest
        {
            public List<CustomizeDayRequest>? Days { get; set; }
        }

        public class CustomizeDayRequest
        {
            public long InstanceDayId { get; set; } // Reference to the instance itinerary day
            public string? Title { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool? BreakfastIncluded { get; set; }
            public bool? LunchIncluded { get; set; }
            public bool? DinnerIncluded { get; set; }
            public List<string>? Activities { get; set; }
        }

        // ===========================
        // Request to update booking info
        // ===========================
        public class UpdateBookingRequest
        {
            // Allow changing traveler info (for first traveler)
            public string? TravelerName { get; set; }
            public string? TravelerEmail { get; set; }
            public string? TravelerPhone { get; set; }

            // Update group size
            public int? Adults { get; set; }
            public int? Children { get; set; }
            public int? Seniors { get; set; }

            // Update selected days or customize activities/meals per day
            public List<UpdateBookingDayRequest>? Days { get; set; }

            // Optional notes / special requests
            public string? SpecialRequests { get; set; }
        }

        // ===========================
        // Update details for each booking day
        // ===========================
        public class UpdateBookingDayRequest
        {
            public long DayId { get; set; } // Reference to itinerary instance day
            public List<string>? Activities { get; set; } // Updated activities
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }

            // Meals
            public bool? BreakfastIncluded { get; set; }
            public bool? LunchIncluded { get; set; }
            public bool? DinnerIncluded { get; set; }
        }

        // ===========================
        // Booking response
        // ===========================
        public class BookingResponse
        {
            public long InstanceId { get; set; }
            public string Status { get; set; } = "Draft"; // Draft, Pending, Approved, Completed
            public decimal TotalAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal BalanceAmount { get; set; }
        }

        // ===========================
        // Booking detail response
        // ===========================
        public class BookingDetailResponse : BookingResponse
        {
            public long TemplateId { get; set; } // Reference to the template itinerary
            public string TemplateTitle { get; set; } = string.Empty;
            public List<ItineraryDayResponse> Days { get; set; } = new();
            public List<TravelerRequest> Travelers { get; set; } = new();
            public string? SpecialRequests { get; set; }
        }

        // ===========================
        // Payment info
        // ===========================
        public class AddPaymentRequest
        {
            public decimal Amount { get; set; }
            public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
            public string? PaymentMethod { get; set; } // e.g., CreditCard, BankTransfer
        }

        public class PaymentResponse
        {
            public long PaymentId { get; set; }
            public decimal Amount { get; set; }
            public DateTime PaymentDate { get; set; }
            public string PaymentMethod { get; set; } = string.Empty;
        }
        public class BookingDayResponse
        {
            public long Id { get; set; }                  // BookingDay table PK
            public long BookingId { get; set; }           // Reference to the Booking
            public string? Title { get; set; }            // Day title
            public string? Location { get; set; }         // Location of the day
            public string? Accommodation { get; set; }   // Hotel/Stay
            public string? Transport { get; set; }       // Transport mode
            public bool BreakfastIncluded { get; set; }  // Meal flags
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public List<string> Activities { get; set; } = new(); // Activities for the day
            public decimal TotalFee { get; set; }        // Total fee for this day (calculated)
        }
    }
}

