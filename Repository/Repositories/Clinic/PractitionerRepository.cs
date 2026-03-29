using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Clinic;
using static Repository.DataModels.Clinic.PractitionerDTO;

namespace Repository.Repositories.Clinic
{
    public class PractitionerRepository : IPractitionerRepository
    {
        private readonly IDbConnection _dbConnection;

        public PractitionerRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public PractitionerResponse Create(CreatePractitionerRequest request)
        {
            string sql = @"
                INSERT INTO practitioners (tenant_id, first_name, last_name, email, phone, specialization, bio, role, created_at)
                VALUES (@TenantId, @FirstName, @LastName, @Email, @Phone, @Specialization, @Bio, @Role, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(request.TenantId, id)!;
        }

        public List<PractitionerResponse> GetByTenant(long tenantId)
        {
            string sql = @"
                SELECT id, tenant_id, first_name, last_name, email, phone, specialization, bio, role, is_active, created_at, updated_at
                FROM practitioners
                WHERE tenant_id = @TenantId
                ORDER BY first_name, last_name;";
            return _dbConnection.Query<PractitionerResponse>(sql, new { TenantId = tenantId }).ToList();
        }

        public PractitionerResponse? GetById(long tenantId, long id)
        {
            string sql = @"
                SELECT id, tenant_id, first_name, last_name, email, phone, specialization, bio, role, is_active, created_at, updated_at
                FROM practitioners
                WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.QuerySingleOrDefault<PractitionerResponse>(sql, new { Id = id, TenantId = tenantId });
        }

        public PractitionerResponse? Update(long tenantId, long id, UpdatePractitionerRequest request)
        {
            string sql = @"
                UPDATE practitioners
                SET first_name = COALESCE(@FirstName, first_name),
                    last_name = COALESCE(@LastName, last_name),
                    email = COALESCE(@Email, email),
                    phone = COALESCE(@Phone, phone),
                    specialization = COALESCE(@Specialization, specialization),
                    bio = COALESCE(@Bio, bio),
                    role = COALESCE(@Role, role),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id AND tenant_id = @TenantId;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.FirstName,
                request.LastName,
                request.Email,
                request.Phone,
                request.Specialization,
                request.Bio,
                request.Role,
                request.IsActive,
                Id = id,
                TenantId = tenantId
            });

            return affected > 0 ? GetById(tenantId, id) : null;
        }

        public bool Delete(long tenantId, long id)
        {
            string sql = "DELETE FROM practitioners WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.Execute(sql, new { Id = id, TenantId = tenantId }) > 0;
        }
    }
}
