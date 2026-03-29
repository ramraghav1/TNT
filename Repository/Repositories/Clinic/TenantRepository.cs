using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Clinic;
using static Repository.DataModels.Clinic.TenantDTO;

namespace Repository.Repositories.Clinic
{
    public class TenantRepository : ITenantRepository
    {
        private readonly IDbConnection _dbConnection;

        public TenantRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public TenantResponse Create(CreateTenantRequest request)
        {
            string sql = @"
                INSERT INTO tenants (name, slug, email, phone, address, logo_url, timezone, created_at)
                VALUES (@Name, @Slug, @Email, @Phone, @Address, @LogoUrl, @Timezone, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(id)!;
        }

        public List<TenantResponse> GetAll()
        {
            string sql = @"
                SELECT id, name, slug, email, phone, address, logo_url, timezone, is_active, created_at, updated_at
                FROM tenants
                ORDER BY name;";
            return _dbConnection.Query<TenantResponse>(sql).ToList();
        }

        public TenantResponse? GetById(long id)
        {
            string sql = @"
                SELECT id, name, slug, email, phone, address, logo_url, timezone, is_active, created_at, updated_at
                FROM tenants
                WHERE id = @Id;";
            return _dbConnection.QuerySingleOrDefault<TenantResponse>(sql, new { Id = id });
        }

        public TenantResponse? GetBySlug(string slug)
        {
            string sql = @"
                SELECT id, name, slug, email, phone, address, logo_url, timezone, is_active, created_at, updated_at
                FROM tenants
                WHERE slug = @Slug;";
            return _dbConnection.QuerySingleOrDefault<TenantResponse>(sql, new { Slug = slug });
        }

        public TenantResponse? Update(long id, UpdateTenantRequest request)
        {
            string sql = @"
                UPDATE tenants
                SET name = COALESCE(@Name, name),
                    email = COALESCE(@Email, email),
                    phone = COALESCE(@Phone, phone),
                    address = COALESCE(@Address, address),
                    logo_url = COALESCE(@LogoUrl, logo_url),
                    timezone = COALESCE(@Timezone, timezone),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Name,
                request.Email,
                request.Phone,
                request.Address,
                request.LogoUrl,
                request.Timezone,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM tenants WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
