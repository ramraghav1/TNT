using System;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Bussiness.Services
{
    /// <summary>
    /// Implementation that extracts tenant ID from JWT claims
    /// Implements the interface from Repository layer to avoid circular dependencies
    /// </summary>
    public class JwtTenantProvider : Repository.Repositories.ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? GetTenantId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || user.Identity?.IsAuthenticated != true)
                return null;

            // Extract TenantId from JWT claims
            var tenantIdClaim = user.FindFirst("TenantId") ?? user.FindFirst(ClaimTypes.GroupSid);
            
            if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out long tenantId))
            {
                return tenantId;
            }

            return null;
        }

        public long GetRequiredTenantId()
        {
            var tenantId = GetTenantId();
            if (tenantId == null)
            {
                throw new UnauthorizedAccessException("Tenant context is required but not available. Ensure the user is authenticated with a valid tenant.");
            }
            return tenantId.Value;
        }
    }
}
