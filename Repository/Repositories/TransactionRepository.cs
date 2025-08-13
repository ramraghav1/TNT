using System;
using System.Data;
using System.Transactions;
using Dapper;
using Repository.DataModels.Transaction;
using Repository.Interfaces;

namespace Repository.Repositories
{
	public class TransactionRepository:ITransactionRepository
	{
        private readonly IDbConnection _dbConnection;
        public TransactionRepository(IDbConnection dbConnection)
		{
			_dbConnection = dbConnection;
		}

        public ReturnCreateTransactionDTO CreateTransaction(CreateTransactionDTO objReturnCreateTransactionDTO)
        {
            if (_dbConnection.State != System.Data.ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                int transaction_id = 0;
                try
                {
                    string insertTransactionQuery = @"
                                                    INSERT INTO transaction_detail (
                                                        payment_type,
                                                        payout_location,
                                                        collected_amount,
                                                        service_fee,
                                                        transfer_amount,
                                                        sender_name,
                                                        sender_address,
                                                        sender_mobile,
                                                        receiver_name,
                                                        receiver_address,
                                                        receiver_mobile
                                                    )
                                                    VALUES (
                                                        @payment_type,
                                                        @payout_location,
                                                        @collected_amount,
                                                        @service_fee,
                                                        @transfer_amount,
                                                        @sender_name,
                                                        @sender_address,
                                                        @sender_mobile,
                                                        @receiver_name,
                                                        @receiver_address,
                                                        @receiver_mobile
                                                    )RETURNING transaction_id";
                    transaction_id = _dbConnection.QuerySingle<int>(insertTransactionQuery, objReturnCreateTransactionDTO, transaction);
                    transaction.Commit();
                    return new ReturnCreateTransactionDTO
                    {
                        transaction_id = transaction_id

                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public List<TransactionDetailDTO> GetTransactionList()
        {
            string query = "SELECT * FROM transaction_detail;";
            var resultList = _dbConnection.Query<TransactionDetailDTO>(query).ToList();
            return resultList;
        }
    }
}

