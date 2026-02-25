using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.AgentLedgerEntryDTO;

namespace Repository.Repositories.Remittance
{
    public class AgentLedgerEntryRepository : IAgentLedgerEntryRepository
    {
        private readonly IDbConnection _dbConnection;

        public AgentLedgerEntryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public LedgerEntryResponse Create(CreateLedgerEntryRequest request, decimal balanceBefore, decimal balanceAfter)
        {
            string sql = @"
                INSERT INTO agent_ledger_entries
                    (agent_account_id, transaction_type, amount, balance_before, balance_after,
                     reference_type, reference_id, description, created_at, created_by)
                VALUES
                    (@AgentAccountId, @TransactionType, @Amount, @BalanceBefore, @BalanceAfter,
                     @ReferenceType, @ReferenceId, @Description, NOW(), @CreatedBy)
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, new
            {
                request.AgentAccountId,
                request.TransactionType,
                request.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                request.ReferenceType,
                request.ReferenceId,
                request.Description,
                request.CreatedBy
            });

            return GetById(id)!;
        }

        public List<LedgerEntryResponse> GetByAccount(long agentAccountId)
        {
            string sql = @"
                SELECT id, agent_account_id, transaction_type, amount,
                       balance_before, balance_after, reference_type, reference_id,
                       description, created_at, created_by
                FROM agent_ledger_entries
                WHERE agent_account_id = @AgentAccountId
                ORDER BY created_at DESC;";
            return _dbConnection.Query<LedgerEntryResponse>(sql, new { AgentAccountId = agentAccountId }).ToList();
        }

        public List<LedgerEntryResponse> GetByAccountAndDateRange(long agentAccountId, DateTime? fromDate, DateTime? toDate)
        {
            string sql = @"
                SELECT id, agent_account_id, transaction_type, amount,
                       balance_before, balance_after, reference_type, reference_id,
                       description, created_at, created_by
                FROM agent_ledger_entries
                WHERE agent_account_id = @AgentAccountId
                  AND (@FromDate IS NULL OR created_at >= @FromDate)
                  AND (@ToDate   IS NULL OR created_at <= @ToDate)
                ORDER BY created_at ASC;";
            return _dbConnection.Query<LedgerEntryResponse>(sql, new
            {
                AgentAccountId = agentAccountId,
                FromDate = fromDate,
                ToDate = toDate
            }).ToList();
        }

        public LedgerEntryResponse? GetById(long id)
        {
            string sql = @"
                SELECT id, agent_account_id, transaction_type, amount,
                       balance_before, balance_after, reference_type, reference_id,
                       description, created_at, created_by
                FROM agent_ledger_entries
                WHERE id = @Id;";
            return _dbConnection.QuerySingleOrDefault<LedgerEntryResponse>(sql, new { Id = id });
        }
    }
}
