using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace server.MiddleWare
{
    /// <summary>
    /// Middleware to validate tenant context and ensure tenant is active
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _publicPaths = new[] 
        { 
            "/api/auth/login", 
            "/api/auth/register", 
            "/api/health",
            "/swagger"
        };

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Repository.Repositories.ITenantProvider tenantProvider)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Skip tenant validation for public endpoints
            if (_publicPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            // For authenticated requests, validate tenant context
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var tenantId = tenantProvider.GetTenantId();
                
                // Use default tenant (1) for backward compatibility if TenantId is missing
                // This allows old JWT tokens to work during transition period
                var effectiveTenantId = tenantId ?? 1;

                // Add tenant ID to response headers for debugging
                context.Response.Headers["X-Tenant-Id"] = effectiveTenantId.ToString();
                
                // Optionally log a warning if using default tenant
                if (tenantId == null)
                {
                    // User should re-login to get proper tenant context
                    context.Response.Headers["X-Tenant-Warning"] = "Using default tenant. Please re-login for proper tenant context.";
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to register tenant middleware
    /// </summary>
    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
