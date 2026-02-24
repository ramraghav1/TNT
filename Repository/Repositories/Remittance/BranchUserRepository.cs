using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.BranchUserDTO;

namespace Repository.Repositories.Remittance
{
    public class BranchUserRepository : IBranchUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public BranchUserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public BranchUserResponse Create(CreateBranchUserRequest request)
        {
            string sql = @"
                INSERT INTO branch_users (branch_id, full_name, email, phone, role, username, created_at)
                VALUES (@BranchId, @FullName, @Email, @Phone, @Role, @Username, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(id)!;
        }

        public List<BranchUserResponse> GetByBranch(long branchId)
        {
            string sql = @"
                SELECT bu.id, bu.branch_id, b.branch_name,
                       bu.full_name, bu.email, bu.phone, bu.role, bu.username,
                       bu.is_active, bu.created_at
                FROM branch_users bu
                JOIN branches b ON b.id = bu.branch_id
                WHERE bu.branch_id = @BranchId
                ORDER BY bu.full_name;";
            return _dbConnection.Query<BranchUserResponse>(sql, new { BranchId = branchId }).ToList();
        }

        public BranchUserResponse? GetById(long id)
        {
            string sql = @"
                SELECT bu.id, bu.branch_id, b.branch_name,
                       bu.full_name, bu.email, bu.phone, bu.role, bu.username,
                       bu.is_active, bu.created_at
                FROM branch_users bu
                JOIN branches b ON b.id = bu.branch_id
                WHERE bu.id = @Id;";
            return _dbConnection.QuerySingleOrDefault<BranchUserResponse>(sql, new { Id = id });
        }

        public BranchUserResponse? Update(long id, UpdateBranchUserRequest request)
        {
            string sql = @"
                UPDATE branch_users
                SET full_name = COALESCE(@FullName, full_name),
                    email = COALESCE(@Email, email),
                    phone = COALESCE(@Phone, phone),
                    role = COALESCE(@Role, role),
                    username = COALESCE(@Username, username),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.FullName,
                request.Email,
                request.Phone,
                request.Role,
                request.Username,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM branch_users WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
