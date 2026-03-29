using System;
namespace Domain.Models.Transaction
{
	public class CreateTransactionModel
	{
        public string? PaymentType { get; set; }
        public string? PayoutLocation { get; set; }
        public decimal? CollectedAmount { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? TransferAmount { get; set; }

        // Sender
        public string? SenderName { get; set; }
        public string? SenderAddress { get; set; }
        public string? SenderMobile { get; set; }

        // Receiver
        public string? ReceiverName { get; set; }
        public string? ReceiverAddress { get; set; }
        public string? ReceiverMobile { get; set; }

        // Agent & Branch
        public long? SenderAgentId { get; set; }
        public long? SenderBranchId { get; set; }
        public long? PayoutAgentId { get; set; }
        public long? PayoutBranchId { get; set; }
    }
    public class ReturnCreateTransactionModel
    {
        public int TransactionId { get; set; }
        public string? ReferenceNumber { get; set; }

    }
}

