using System;
namespace Repository.DataModels.Transaction
{
    public class CreateTransactionDTO
    {
        // Transaction
        public string? payment_type { get; set; }
        public string? payout_location { get; set; }
        public decimal? collected_amount { get; set; }
        public decimal? service_fee { get; set; }
        public decimal? transfer_amount { get; set; }

        // Sender
        public string? sender_name { get; set; }
        public string? sender_address { get; set; }
        public string? sender_mobile { get; set; }

        // Receiver
        public string? receiver_name { get; set; }
        public string? receiver_address { get; set; }
        public string? receiver_mobile { get; set; }
    }
    public class ReturnCreateTransactionDTO
    {
        public int transaction_id { get; set; }
        public string? reference_number { get; set; }
   
    }

}

