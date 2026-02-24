using System;

namespace Repository.DataModels
{
    public class AuthTokenDTO
    {
        // ===========================
        // Store a new token
        // ===========================
        public class CreateAuthTokenRequest
        {
            public int UserId { get; set; }
            public string RefreshToken { get; set; } = string.Empty;
            public string? AccessToken { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string? IpAddress { get; set; }
            public string? UserAgent { get; set; }
        }

        // ===========================
        // Token record from DB
        // ===========================
        public class AuthTokenResponse
        {
            public int TokenId { get; set; }
            public int UserId { get; set; }
            public string RefreshToken { get; set; } = string.Empty;
            public string? AccessToken { get; set; }
            public DateTime IssuedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public bool Revoked { get; set; }
            public DateTime? RevokedAt { get; set; }
            public string? IpAddress { get; set; }
            public string? UserAgent { get; set; }
        }

        // ===========================
        // Validated user from login check
        // ===========================
        public class ValidatedUserResponse
        {
            public int LoginId { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string PasswordHash { get; set; } = string.Empty;
            public string UserFullName { get; set; } = string.Empty;
            public string? EmailAddress { get; set; }
            public string? MobileNumber { get; set; }
        }

    }
}
