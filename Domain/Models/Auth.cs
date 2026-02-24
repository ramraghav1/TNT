using System;

namespace Domain.Models
{
    public class Auth
    {
        // ===========================
        // Login Request / Response
        // ===========================
        public class AuthLoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string? IpAddress { get; set; }
            public string? UserAgent { get; set; }
        }

        public class AuthLoginResponse
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime AccessTokenExpiresAt { get; set; }
            public DateTime RefreshTokenExpiresAt { get; set; }
            public UserInfo User { get; set; } = new();
        }

        public class UserInfo
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string UserFullName { get; set; } = string.Empty;
            public string? EmailAddress { get; set; }
            public string? MobileNumber { get; set; }
        }

        // ===========================
        // Refresh Token Request / Response
        // ===========================
        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; } = string.Empty;
            public string? IpAddress { get; set; }
            public string? UserAgent { get; set; }
        }

        public class RefreshTokenResponse
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime AccessTokenExpiresAt { get; set; }
            public DateTime RefreshTokenExpiresAt { get; set; }
        }

        // ===========================
        // Logout Request
        // ===========================
        public class LogoutRequest
        {
            public string RefreshToken { get; set; } = string.Empty;
        }

        // ===========================
        // Auth Token (stored token record)
        // ===========================
        public class AuthTokenRecord
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
        // Validated User (from DB login check)
        // ===========================
        public class ValidatedUser
        {
            public int LoginId { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string UserFullName { get; set; } = string.Empty;
            public string? EmailAddress { get; set; }
            public string? MobileNumber { get; set; }
        }
    }
}
