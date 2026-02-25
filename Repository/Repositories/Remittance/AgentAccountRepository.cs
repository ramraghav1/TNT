using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.AgentAccountDTO;

namespace Repository.Repositories.Remittance
{
    public class AgentAccountRepository : IAgentAccountRepository
    {
        private readonly IDbConnection _dbConnection;

        public AgentAccountRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public AgentAccountResponse Create(CreateAgentAccountRequest request)
        {
            string sql = @"
                INSERT INTO agent_accounts
                    (agent_id, account_name, account_number, bank_name, bank_branch, bank_details, currency_code, balance, created_at)
                VALUES
                    (@AgentId, @AccountName, @AccountNumber, @BankName, @BankBranch, @BankDetails, @CurrencyCode, @Balance, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(id)!;
        }

        public List<AgentAccountResponse> GetAll()
        {
            string sql = @"
                SELECT aa.id, aa.agent_id, a.name AS agent_name,
                       aa.account_name, aa.account_number,
                       aa.bank_name, aa.bank_branch, aa.bank_details,
                       aa.currency_code, aa.balance, aa.is_active,
                       aa.created_at, aa.updated_at
                FROM agent_accounts aa
                JOIN agents a ON a.id = aa.agent_id
                ORDER BY a.name, aa.currency_code;";
            return _dbConnection.Query<AgentAccountResponse>(sql).ToList();
        }

        public List<AgentAccountResponse> GetByAgent(long agentId)
        {
            string sql = @"
                SELECT aa.id, aa.agent_id, a.name AS agent_name,
                       aa.account_name, aa.account_number,
                       aa.bank_name, aa.bank_branch, aa.bank_details,
                       aa.currency_code, aa.balance, aa.is_active,
                       aa.created_at, aa.updated_at
                FROM agent_accounts aa
                JOIN agents a ON a.id = aa.agent_id
                WHERE aa.agent_id = @AgentId
                ORDER BY aa.currency_code;";
            return _dbConnection.Query<AgentAccountResponse>(sql, new { AgentId = agentId }).ToList();
        }

        public AgentAccountResponse? GetById(long id)
        {
            string sql = @"
                SELECT aa.id, aa.agent_id, a.name AS agent_name,
                       aa.account_name, aa.account_number,
                       aa.bank_name, aa.bank_branch, aa.bank_details,
                       aa.currency_code, aa.balance, aa.is_active,
                       aa.created_at, aa.updated_at
                FROM agent_accounts aa
                JOIN agents a ON a.id = aa.agent_id
                WHERE aa.id = @Id;";
            return _dbConnection.QuerySingleOrDefault<AgentAccountResponse>(sql, new { Id = id });
        }

        public AgentAccountResponse? GetByAgentAndCurrency(long agentId, string currencyCode)
        {
            string sql = @"
                SELECT aa.id, aa.agent_id, a.name AS agent_name,
                       aa.account_name, aa.account_number,
                       aa.bank_name, aa.bank_branch, aa.bank_details,
                       aa.currency_code, aa.balance, aa.is_active,
                       aa.created_at, aa.updated_at
                FROM agent_accounts aa
                JOIN agents a ON a.id = aa.agent_id
                WHERE aa.agent_id = @AgentId AND aa.currency_code = @CurrencyCode;";
            return _dbConnection.QuerySingleOrDefault<AgentAccountResponse>(sql, new { AgentId = agentId, CurrencyCode = currencyCode });
        }

        public AgentAccountResponse? Update(long id, UpdateAgentAccountRequest request)
        {
            string sql = @"
                UPDATE agent_accounts
                SET account_name   = COALESCE(@AccountName, account_name),
                    account_number = COALESCE(@AccountNumber, account_number),
                    bank_name      = COALESCE(@BankName, bank_name),
                    bank_branch    = COALESCE(@BankBranch, bank_branch),
                    bank_details   = COALESCE(@BankDetails, bank_details),
                    is_active      = COALESCE(@IsActive, is_active),
                    updated_at     = NOW()
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.AccountName,
                request.AccountNumber,
                request.BankName,
                request.BankBranch,
                request.BankDetails,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool UpdateBalance(long id, decimal newBalance)
        {
            string sql = @"
                UPDATE agent_accounts
                SET balance = @Balance, updated_at = NOW()
                WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Balance = newBalance, Id = id }) > 0;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM agent_accounts WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
