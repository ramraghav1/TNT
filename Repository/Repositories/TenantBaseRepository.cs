using System.Data;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Repository.Repositories
{
    /// <summary>
    /// Interface for getting tenant context from the current request
    /// </summary>
    public interface ITenantProvider
    {
        long? GetTenantId();
        long GetRequiredTenantId();
    }

    /// <summary>
    /// Base repository with tenant-aware query capabilities
    /// </summary>
    public abstract class TenantBaseRepository
    {
        protected readonly IDbConnection _dbConnection;
        protected readonly ITenantProvider _tenantProvider;

        protected TenantBaseRepository(IDbConnection dbConnection, ITenantProvider tenantProvider)
        {
            _dbConnection = dbConnection;
            _tenantProvider = tenantProvider;
        }

        /// <summary>
        /// Gets the current tenant ID. Throws if not available.
        /// </summary>
        protected long GetTenantId()
        {
            return _tenantProvider.GetRequiredTenantId();
        }

        /// <summary>
        /// Adds tenant_id filter to WHERE clause
        /// Usage: WHERE {GetTenantFilter("table_alias")}
        /// </summary>
        protected string GetTenantFilter(string? tableAlias = null)
        {
            var prefix = string.IsNullOrEmpty(tableAlias) ? "" : $"{tableAlias}.";
            return $"{prefix}tenant_id = @TenantId";
        }

        /// <summary>
        /// Creates DynamicParameters with TenantId already included
        /// </summary>
        protected DynamicParameters CreateTenantParameters(object? additionalParams = null)
        {
            var parameters = new DynamicParameters(additionalParams);
            parameters.Add("TenantId", GetTenantId());
            return parameters;
        }

        /// <summary>
        /// Validates that a record belongs to the current tenant
        /// Throws UnauthorizedAccessException if tenant doesn't match
        /// </summary>
        protected void ValidateTenantOwnership(long recordTenantId, string resourceName = "Resource")
        {
            var currentTenantId = GetTenantId();
            if (recordTenantId != currentTenantId)
            {
                throw new UnauthorizedAccessException(
                    $"{resourceName} does not belong to your tenant. Access denied.");
            }
        }

        /// <summary>
        /// Executes a query with automatic tenant filtering
        /// </summary>
        protected IEnumerable<T> QueryWithTenant<T>(
            string sql, 
            object? parameters = null, 
            IDbTransaction? transaction = null)
        {
            var tenantParams = CreateTenantParameters(parameters);
            return _dbConnection.Query<T>(sql, tenantParams, transaction);
        }

        /// <summary>
        /// Executes a single query with automatic tenant filtering
        /// </summary>
        protected T? QuerySingleOrDefaultWithTenant<T>(
            string sql, 
            object? parameters = null, 
            IDbTransaction? transaction = null)
        {
            var tenantParams = CreateTenantParameters(parameters);
            return _dbConnection.QuerySingleOrDefault<T>(sql, tenantParams, transaction);
        }

        /// <summary>
        /// Executes a command with automatic tenant parameter
        /// </summary>
        protected int ExecuteWithTenant(
            string sql, 
            object? parameters = null, 
            IDbTransaction? transaction = null)
        {
            var tenantParams = CreateTenantParameters(parameters);
            return _dbConnection.Execute(sql, tenantParams, transaction);
        }
    }
}
