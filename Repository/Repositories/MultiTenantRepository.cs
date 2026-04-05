using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Domain.Models;
using Repository.DataModels;
using Newtonsoft.Json;

namespace Repository.Repositories
{
    public interface IMultiTenantRepository
    {
        Tenant? GetTenantById(long id);
        Tenant? GetTenantBySubdomain(string subdomain);
        List<Tenant> GetAllTenants();
        long CreateTenant(string name, string subdomain, string contactEmail, string? contactPhone = null, TenantSettings? settings = null);
        bool UpdateTenant(long id, string name, string? logoUrl, string? contactPhone);
        bool ActivateTenantProduct(long tenantId, int productId, string? subscriptionTier = null);
        bool DeactivateTenantProduct(long tenantId, int productId);
        List<string> GetTenantProducts(long tenantId);
        List<ProductDTO> GetAllProducts();
    }

    public class MultiTenantRepository : IMultiTenantRepository
    {
        private readonly IDbConnection _dbConnection;

        public MultiTenantRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Tenant? GetTenantById(long id)
        {
            string query = @"
                SELECT id, name, subdomain, logo_url, contact_email, contact_phone, 
                       status, settings_json, created_at, updated_at
                FROM saas_tenants
                WHERE id = @Id;";

            var dto = _dbConnection.QuerySingleOrDefault<TenantDTO>(query, new { Id = id });
            return MapToTenant(dto);
        }

        public Tenant? GetTenantBySubdomain(string subdomain)
        {
            string query = @"
                SELECT id, name, subdomain, logo_url, contact_email, contact_phone, 
                       status, settings_json, created_at, updated_at
                FROM saas_tenants
                WHERE subdomain = @Subdomain;";

            var dto = _dbConnection.QuerySingleOrDefault<TenantDTO>(query, new { Subdomain = subdomain });
            return MapToTenant(dto);
        }

        public List<Tenant> GetAllTenants()
        {
            string query = @"
                SELECT id, name, subdomain, logo_url, contact_email, contact_phone, 
                       status, settings_json, created_at, updated_at
                FROM saas_tenants
                ORDER BY created_at DESC;";

            var dtos = _dbConnection.Query<TenantDTO>(query).ToList();
            return dtos.Select(MapToTenant).Where(t => t != null).Cast<Tenant>().ToList();
        }

        public long CreateTenant(string name, string subdomain, string contactEmail, string? contactPhone = null, TenantSettings? settings = null)
        {
            var settingsJson = settings != null ? JsonConvert.SerializeObject(settings) : null;
            
            string insertQuery = @"
                INSERT INTO saas_tenants (name, subdomain, contact_email, contact_phone, status, settings_json)
                VALUES (@Name, @Subdomain, @ContactEmail, @ContactPhone, 'Active', @SettingsJson::jsonb)
                RETURNING id;";

            return _dbConnection.QuerySingle<long>(
                insertQuery,
                new { Name = name, Subdomain = subdomain, ContactEmail = contactEmail, ContactPhone = contactPhone, SettingsJson = settingsJson }
            );
        }

        public bool UpdateTenant(long id, string name, string? logoUrl, string? contactPhone)
        {
            string updateQuery = @"
                UPDATE saas_tenants
                SET name = @Name,
                    logo_url = @LogoUrl,
                    contact_phone = @ContactPhone,
                    updated_at = NOW()
                WHERE id = @Id;";

            int rowsAffected = _dbConnection.Execute(
                updateQuery,
                new { Id = id, Name = name, LogoUrl = logoUrl, ContactPhone = contactPhone }
            );

            return rowsAffected > 0;
        }

        public bool ActivateTenantProduct(long tenantId, int productId, string? subscriptionTier = null)
        {
            string query = @"
                INSERT INTO tenant_products (tenant_id, product_id, is_active, subscription_tier)
                VALUES (@TenantId, @ProductId, true, @SubscriptionTier)
                ON CONFLICT (tenant_id, product_id) 
                DO UPDATE SET is_active = true, activated_at = NOW(), deactivated_at = NULL;";

            int rowsAffected = _dbConnection.Execute(
                query,
                new { TenantId = tenantId, ProductId = productId, SubscriptionTier = subscriptionTier }
            );

            return rowsAffected > 0;
        }

        public bool DeactivateTenantProduct(long tenantId, int productId)
        {
            string query = @"
                UPDATE tenant_products
                SET is_active = false, deactivated_at = NOW()
                WHERE tenant_id = @TenantId AND product_id = @ProductId;";

            int rowsAffected = _dbConnection.Execute(
                query,
                new { TenantId = tenantId, ProductId = productId }
            );

            return rowsAffected > 0;
        }

        public List<string> GetTenantProducts(long tenantId)
        {
            string query = @"
                SELECT p.name
                FROM tenant_products tp
                JOIN products p ON p.id = tp.product_id
                WHERE tp.tenant_id = @TenantId AND tp.is_active = true;";

            return _dbConnection.Query<string>(query, new { TenantId = tenantId }).ToList();
        }

        public List<ProductDTO> GetAllProducts()
        {
            string query = @"
                SELECT id, name, display_name, description, is_active
                FROM products
                ORDER BY id;";

            return _dbConnection.Query<ProductDTO>(query).ToList();
        }

        private Tenant? MapToTenant(TenantDTO? dto)
        {
            if (dto == null) return null;

            var settings = string.IsNullOrEmpty(dto.SettingsJson)
                ? new TenantSettings()
                : JsonConvert.DeserializeObject<TenantSettings>(dto.SettingsJson) ?? new TenantSettings();

            return new Tenant
            {
                Id = dto.Id,
                Name = dto.Name,
                Subdomain = dto.Subdomain,
                LogoUrl = dto.LogoUrl,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                Status = dto.Status,
                Settings = settings,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }
    }
}
