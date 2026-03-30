using Repository.Interfaces;

namespace server.MiddleWare
{
    /// <summary>
    /// Middleware that checks if the bearer token in the request has been revoked.
    /// Runs after UseAuthentication() so that the JWT is already validated.
    /// If the token is revoked, returns 401 Unauthorized.
    /// </summary>
    public class TokenRevocationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRevocationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthTokenRepository tokenRepo)
        {
            // Only check authenticated requests
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Extract the raw bearer token from the Authorization header
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var accessToken = authHeader.Substring("Bearer ".Length).Trim();

                    // Check if this token has been revoked in the DB
                    if (tokenRepo.IsAccessTokenRevoked(accessToken))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"success\":false,\"message\":\"Token has been revoked\"}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
