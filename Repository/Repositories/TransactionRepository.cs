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
                                                        receiver_mobile,
                                                        sender_agent_id,
                                                        sender_branch_id,
                                                        payout_agent_id,
                                                        payout_branch_id,
                                                        status,
                                                        created_at
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
                                                        @receiver_mobile,
                                                        @sender_agent_id,
                                                        @sender_branch_id,
                                                        @payout_agent_id,
                                                        @payout_branch_id,
                                                        'SENT',
                                                        NOW()
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
            string query = @"
                SELECT t.transaction_id, t.reference_number, t.payment_type, t.payout_location,
                       t.collected_amount, t.service_fee, t.transfer_amount,
                       t.sender_name, t.sender_address, t.sender_mobile,
                       t.receiver_name, t.receiver_address, t.receiver_mobile,
                       t.sender_agent_id, sa.name AS sender_agent_name,
                       t.sender_branch_id, sb.branch_name AS sender_branch_name,
                       t.payout_agent_id, pa.name AS payout_agent_name,
                       t.payout_branch_id, pb.branch_name AS payout_branch_name,
                       t.status, t.created_at
                FROM transaction_detail t
                LEFT JOIN agents sa ON sa.id = t.sender_agent_id
                LEFT JOIN branches sb ON sb.id = t.sender_branch_id
                LEFT JOIN agents pa ON pa.id = t.payout_agent_id
                LEFT JOIN branches pb ON pb.id = t.payout_branch_id
                ORDER BY t.transaction_id DESC;";
            var resultList = _dbConnection.Query<TransactionDetailDTO>(query).ToList();
            return resultList;
        }
    }
}

