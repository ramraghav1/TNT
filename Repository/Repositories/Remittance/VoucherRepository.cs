using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.VoucherDTO;

namespace Repository.Repositories.Remittance
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly IDbConnection _dbConnection;

        public VoucherRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        private const string SelectColumns = @"
            v.id, v.voucher_number, v.voucher_date, v.entity_type,
            v.agent_id, ag.name AS agent_name,
            v.branch_id, br.branch_name AS branch_name,
            v.agent_account_id,
            v.amount, v.mode, v.balance_before, v.balance_after,
            v.reference_type, v.reference_id, v.description,
            v.created_at, v.created_by";

        private const string FromJoins = @"
            FROM vouchers v
            LEFT JOIN agents ag ON ag.id = v.agent_id
            LEFT JOIN branches br ON br.id = v.branch_id";

        public VoucherResponse Create(CreateVoucherRequest request, decimal balanceBefore, decimal balanceAfter, string voucherNumber)
        {
            string sql = @"
                INSERT INTO vouchers
                    (voucher_number, voucher_date, entity_type, agent_id, branch_id, agent_account_id,
                     amount, mode, balance_before, balance_after,
                     reference_type, reference_id, description, created_at, created_by)
                VALUES
                    (@VoucherNumber, NOW(), @EntityType, @AgentId, @BranchId, @AgentAccountId,
                     @Amount, @Mode, @BalanceBefore, @BalanceAfter,
                     @ReferenceType, @ReferenceId, @Description, NOW(), @CreatedBy)
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, new
            {
                VoucherNumber = voucherNumber,
                request.EntityType,
                request.AgentId,
                request.BranchId,
                request.AgentAccountId,
                request.Amount,
                request.Mode,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                request.ReferenceType,
                request.ReferenceId,
                request.Description,
                request.CreatedBy
            });

            return GetById(id)!;
        }

        public List<VoucherResponse> GetAll()
        {
            string sql = $"SELECT {SelectColumns} {FromJoins} ORDER BY v.created_at DESC;";
            return _dbConnection.Query<VoucherResponse>(sql).ToList();
        }

        public VoucherResponse? GetById(long id)
        {
            string sql = $"SELECT {SelectColumns} {FromJoins} WHERE v.id = @Id;";
            return _dbConnection.QuerySingleOrDefault<VoucherResponse>(sql, new { Id = id });
        }

        public List<VoucherResponse> GetByAgent(long agentId)
        {
            string sql = $"SELECT {SelectColumns} {FromJoins} WHERE v.agent_id = @AgentId ORDER BY v.created_at DESC;";
            return _dbConnection.Query<VoucherResponse>(sql, new { AgentId = agentId }).ToList();
        }

        public List<VoucherResponse> GetByBranch(long branchId)
        {
            string sql = $"SELECT {SelectColumns} {FromJoins} WHERE v.branch_id = @BranchId ORDER BY v.created_at DESC;";
            return _dbConnection.Query<VoucherResponse>(sql, new { BranchId = branchId }).ToList();
        }

        public List<VoucherResponse> GetByAgentAccount(long agentAccountId)
        {
            string sql = $"SELECT {SelectColumns} {FromJoins} WHERE v.agent_account_id = @AgentAccountId ORDER BY v.created_at DESC;";
            return _dbConnection.Query<VoucherResponse>(sql, new { AgentAccountId = agentAccountId }).ToList();
        }

        public List<VoucherResponse> GetByEntityAndDateRange(
            string entityType, long? agentId, long? branchId, long? agentAccountId,
            DateTime? fromDate, DateTime? toDate)
        {
            string sql = $@"SELECT {SelectColumns} {FromJoins}
                WHERE v.entity_type = @EntityType";

            if (agentId.HasValue)
                sql += " AND v.agent_id = @AgentId";
            if (branchId.HasValue)
                sql += " AND v.branch_id = @BranchId";
            if (agentAccountId.HasValue)
                sql += " AND v.agent_account_id = @AgentAccountId";
            if (fromDate.HasValue)
                sql += " AND v.voucher_date >= @FromDate";
            if (toDate.HasValue)
                sql += " AND v.voucher_date < @ToDate::date + INTERVAL '1 day'";

            sql += " ORDER BY v.voucher_date ASC, v.id ASC;";

            return _dbConnection.Query<VoucherResponse>(sql, new
            {
                EntityType = entityType,
                AgentId = agentId,
                BranchId = branchId,
                AgentAccountId = agentAccountId,
                FromDate = fromDate,
                ToDate = toDate
            }).ToList();
        }

        public decimal GetBranchBalance(long branchId)
        {
            string sql = "SELECT current_balance FROM branches WHERE id = @BranchId;";
            return _dbConnection.QuerySingleOrDefault<decimal>(sql, new { BranchId = branchId });
        }

        public bool UpdateBranchBalance(long branchId, decimal newBalance)
        {
            string sql = @"
                UPDATE branches
                SET current_balance = @Balance, updated_at = NOW()
                WHERE id = @BranchId;";
            return _dbConnection.Execute(sql, new { Balance = newBalance, BranchId = branchId }) > 0;
        }
    }
}
