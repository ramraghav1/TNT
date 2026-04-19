using System;
using System.Collections.Generic;

namespace Domain.Models.TourAndTravels
{
    public class ItineraryProposal
    {
        // ===========================
        // CREATE PROPOSAL
        // ===========================
        public class CreateProposalRequest
        {
            public long ItineraryId { get; set; }
            public string TravelerName { get; set; } = string.Empty;
            public string TravelerEmail { get; set; } = string.Empty;
            public string? TravelerPhone { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string? Notes { get; set; }
            public List<ProposalDayRequest> Days { get; set; } = new();
        }

        public class ProposalDayRequest
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
            public List<string> Activities { get; set; } = new();
            public decimal DailyCost { get; set; }
        }

        // ===========================
        // PROPOSAL RESPONSE
        // ===========================
        public class ProposalResponse
        {
            public long Id { get; set; }
            public long ItineraryId { get; set; }
            public string Token { get; set; } = string.Empty;
            public string TravelerName { get; set; } = string.Empty;
            public string TravelerEmail { get; set; } = string.Empty;
            public string? TravelerPhone { get; set; }
            public string ItineraryTitle { get; set; } = string.Empty;
            public string Status { get; set; } = "Sent";
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string? Notes { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public List<ProposalDayResponse> Days { get; set; } = new();
        }

        public class ProposalDayResponse
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
            public List<string> Activities { get; set; } = new();
            public decimal DailyCost { get; set; }
        }

        // ===========================
        // PROPOSAL LIST ITEM
        // ===========================
        public class ProposalListItem
        {
            public long Id { get; set; }
            public string Token { get; set; } = string.Empty;
            public string TravelerName { get; set; } = string.Empty;
            public string TravelerEmail { get; set; } = string.Empty;
            public string ItineraryTitle { get; set; } = string.Empty;
            public string Status { get; set; } = "Sent";
            public decimal TotalAmount { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        // ===========================
        // PAYMENT
        // ===========================
        public class SubmitProposalPaymentRequest
        {
            public string PaymentType { get; set; } = "Full";
            public decimal Amount { get; set; }
            public string? ScreenshotPath { get; set; }
            public string? TransactionReference { get; set; }
        }

        public class ProposalPaymentResponse
        {
            public long Id { get; set; }
            public long ProposalId { get; set; }
            public string PaymentType { get; set; } = "Full";
            public decimal Amount { get; set; }
            public string? ScreenshotPath { get; set; }
            public string? TransactionReference { get; set; }
            public string Status { get; set; } = "Pending";
            public string? VerifiedBy { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? VerifiedAt { get; set; }
        }

        // ===========================
        // FEEDBACK
        // ===========================
        public class SubmitFeedbackRequest
        {
            public string Message { get; set; } = string.Empty;
        }

        public class ProposalFeedbackResponse
        {
            public long Id { get; set; }
            public long ProposalId { get; set; }
            public string Message { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        // ===========================
        // VERIFY PAYMENT
        // ===========================
        public class VerifyPaymentRequest
        {
            public bool Approved { get; set; }
            public string? Remarks { get; set; }
        }
    }
}
