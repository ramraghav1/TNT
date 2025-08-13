using System;
using Repository.DataModels.Transaction;

namespace Repository.Interfaces
{
	public interface ITransactionRepository
	{
        ReturnCreateTransactionDTO CreateTransaction(CreateTransactionDTO objReturnCreateTransactionDTO);
        List<TransactionDetailDTO> GetTransactionList();

    }
}

