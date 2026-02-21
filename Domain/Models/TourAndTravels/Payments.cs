using System;
namespace Domain.Models.TourAndTravels
{
	public class Payments
	{
        public class AddPaymentRequest
        {
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "USD";
            public string PaymentMethod { get; set; } = "Card"; // eSewa, Card, Bank
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
    }
}

