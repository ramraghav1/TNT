using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Clinic;
using static Repository.DataModels.Clinic.ClinicServiceDTO;

namespace Repository.Repositories.Clinic
{
    public class ClinicServiceRepository : IClinicServiceRepository
    {
        private readonly IDbConnection _dbConnection;

        public ClinicServiceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public ClinicServiceResponse Create(CreateClinicServiceRequest request)
        {
            string sql = @"
                INSERT INTO clinic_services (tenant_id, name, description, duration_minutes, price, currency, category, created_at)
                VALUES (@TenantId, @Name, @Description, @DurationMinutes, @Price, @Currency, @Category, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(request.TenantId, id)!;
        }

        public List<ClinicServiceResponse> GetByTenant(long tenantId)
        {
            string sql = @"
                SELECT id, tenant_id, name, description, duration_minutes, price, currency, category, is_active, created_at, updated_at
                FROM clinic_services
                WHERE tenant_id = @TenantId
                ORDER BY category, name;";
            return _dbConnection.Query<ClinicServiceResponse>(sql, new { TenantId = tenantId }).ToList();
        }

        public ClinicServiceResponse? GetById(long tenantId, long id)
        {
            string sql = @"
                SELECT id, tenant_id, name, description, duration_minutes, price, currency, category, is_active, created_at, updated_at
                FROM clinic_services
                WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.QuerySingleOrDefault<ClinicServiceResponse>(sql, new { Id = id, TenantId = tenantId });
        }

        public ClinicServiceResponse? Update(long tenantId, long id, UpdateClinicServiceRequest request)
        {
            string sql = @"
                UPDATE clinic_services
                SET name = COALESCE(@Name, name),
                    description = COALESCE(@Description, description),
                    duration_minutes = COALESCE(@DurationMinutes, duration_minutes),
                    price = COALESCE(@Price, price),
                    currency = COALESCE(@Currency, currency),
                    category = COALESCE(@Category, category),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id AND tenant_id = @TenantId;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Name,
                request.Description,
                request.DurationMinutes,
                request.Price,
                request.Currency,
                request.Category,
                request.IsActive,
                Id = id,
                TenantId = tenantId
            });

            return affected > 0 ? GetById(tenantId, id) : null;
        }

        public bool Delete(long tenantId, long id)
        {
            string sql = "DELETE FROM clinic_services WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.Execute(sql, new { Id = id, TenantId = tenantId }) > 0;
        }
    }
}
