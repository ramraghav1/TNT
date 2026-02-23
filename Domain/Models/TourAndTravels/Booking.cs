using System;

namespace Domain.Models.TourAndTravels
{
    public class Booking
    {
        // ===========================
        // CREATE BOOKING (from Template or from existing Instance)
        // ===========================
        public class CreateBookingRequest
        {
            public long ItineraryId { get; set; }          // Template itinerary ID
            public long? SourceInstanceId { get; set; }    // If cloning from an existing instance (reuse)
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<TravelerRequest> Travelers { get; set; } = new();
            public string? SpecialRequests { get; set; }
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
        // Customize a specific instance day
        // ===========================
        public class CustomizeDayRequest
        {
            public long InstanceDayId { get; set; }
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
        // Booking Response (summary)
        // ===========================
        public class BookingResponse
        {
            public long InstanceId { get; set; }
            public long TemplateId { get; set; }
            public long? SourceInstanceId { get; set; }
            public string? BookingReference { get; set; }
            public string Status { get; set; } = "Draft";
            public bool IsCustomized { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AmountPaid { get; set; }
            public decimal BalanceAmount { get; set; }
            public string PaymentStatus { get; set; } = "Unpaid";
            public bool TravelerApproved { get; set; }
            public bool AdminApproved { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // Booking Detail Response (full with days, travelers, payments)
        // ===========================
        public class BookingDetailResponse : BookingResponse
        {
            public string TemplateTitle { get; set; } = string.Empty;
            public string? SpecialRequests { get; set; }
            public List<BookingDayResponse> Days { get; set; } = new();
            public List<TravelerResponse> Travelers { get; set; } = new();
            public List<PaymentResponse> Payments { get; set; } = new();
        }

        // ===========================
        // Booking Day Response
        // ===========================
        public class BookingDayResponse
        {
            public long Id { get; set; }
            public int DayNumber { get; set; }
            public DateTime? Date { get; set; }
            public string? Title { get; set; }
            public string? Location { get; set; }
            public string? Accommodation { get; set; }
            public string? Transport { get; set; }
            public bool BreakfastIncluded { get; set; }
            public bool LunchIncluded { get; set; }
            public bool DinnerIncluded { get; set; }
            public List<string> Activities { get; set; } = new();
        }

        // ===========================
        // Traveler Response
        // ===========================
        public class TravelerResponse
        {
            public long Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? ContactNumber { get; set; }
            public string? Email { get; set; }
            public string? Nationality { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Seniors { get; set; }
        }

        // ===========================
        // Payment Request & Response
        // ===========================
        public class AddPaymentRequest
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "USD";
            public string PaymentMethod { get; set; } = "Card";
            public string? TransactionReference { get; set; }
        }

        public class PaymentResponse
        {
            public long PaymentId { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "USD";
            public string PaymentMethod { get; set; } = "Card";
            public DateTime PaymentDate { get; set; }
            public string Status { get; set; } = "Success";
            public string? TransactionReference { get; set; }
        }

        // ===========================
        // Approval
        // ===========================
        public class ApproveBookingRequest
        {
            public string ApprovedBy { get; set; } = string.Empty; // "traveler" or "admin"
            public bool Approved { get; set; }
            public string? Remarks { get; set; }
        }

        // ===========================
        // Booking list item (lightweight for lists)
        // ===========================
        public class BookingListItem
        {
            public long InstanceId { get; set; }
            public string? BookingReference { get; set; }
            public string TemplateTitle { get; set; } = string.Empty;
            public string Status { get; set; } = "Draft";
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string PaymentStatus { get; set; } = "Unpaid";
            public DateTime CreatedAt { get; set; }
        }
    }
}

