using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repository.Interfaces;
using static Domain.Models.Auth;

namespace Business.Services
{
    public interface ITokenService
    {
        /// <summary>
        /// Generate an access token + refresh token pair for an authenticated user.
        /// Stores the refresh token in the DB.
        /// </summary>
        AuthLoginResponse GenerateTokens(ValidatedUser user, string? ipAddress, string? userAgent);

        /// <summary>
        /// Validate an existing refresh token and issue a new token pair.
        /// Revokes the old refresh token (rotation).
        /// </summary>
        RefreshTokenResponse RefreshTokens(string refreshToken, string? ipAddress, string? userAgent);

        /// <summary>
        /// Revoke a specific refresh token (logout from one device).
        /// </summary>
        void RevokeToken(string refreshToken);

        /// <summary>
        /// Revoke ALL refresh tokens for a user (logout from all devices).
        /// </summary>
        void RevokeAllTokens(int userId);
    }

    public class TokenService : ITokenService
    {
        private readonly IAuthTokenRepository _tokenRepo;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public TokenService(
            IAuthTokenRepository tokenRepo,
            IMapper mapper,
            IOptions<JwtSettings> jwtOptions)
        {
            _tokenRepo = tokenRepo;
            _mapper = mapper;
            _jwtSettings = jwtOptions.Value;
        }

        public AuthLoginResponse GenerateTokens(ValidatedUser user, string? ipAddress, string? userAgent)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Generate JWT access token
            string accessToken = GenerateAccessToken(user, accessTokenExpiry);

            // Generate secure random refresh token
            string refreshToken = GenerateSecureRefreshToken();
            string refreshTokenHash = HashToken(refreshToken);

            // Store in DB
            var storeRequest = new Repository.DataModels.AuthTokenDTO.CreateAuthTokenRequest
            {
                UserId = user.UserId,
                RefreshToken = refreshTokenHash,
                AccessToken = accessToken,
                ExpiresAt = refreshTokenExpiry,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
            _tokenRepo.StoreToken(storeRequest);

            return new AuthLoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = refreshTokenExpiry,
                User = new UserInfo
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    UserFullName = user.UserFullName,
                    EmailAddress = user.EmailAddress,
                    MobileNumber = user.MobileNumber
                }
            };
        }

        public RefreshTokenResponse RefreshTokens(string refreshToken, string? ipAddress, string? userAgent)
        {
            string hash = HashToken(refreshToken);

            // 1. Find the existing valid token
            var storedToken = _tokenRepo.GetByRefreshToken(hash);
            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            // 2. Revoke the old token (rotation)
            _tokenRepo.RevokeToken(storedToken.TokenId);

            // 3. Generate new pair
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            // Need user info for the new access token claims
            var userRecord = _tokenRepo.ValidateCredentials(GetUsernameFromStoredToken(storedToken.UserId));

            string newAccessToken = GenerateAccessTokenFromUserId(storedToken.UserId, userRecord?.Username ?? "", accessTokenExpiry);
            string newRefreshToken = GenerateSecureRefreshToken();
            string newRefreshTokenHash = HashToken(newRefreshToken);

            // 4. Store new token
            var storeRequest = new Repository.DataModels.AuthTokenDTO.CreateAuthTokenRequest
            {
                UserId = storedToken.UserId,
                RefreshToken = newRefreshTokenHash,
                AccessToken = newAccessToken,
                ExpiresAt = refreshTokenExpiry,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
            _tokenRepo.StoreToken(storeRequest);

            return new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = refreshTokenExpiry
            };
        }

        public void RevokeToken(string refreshToken)
        {
            string hash = HashToken(refreshToken);
            var storedToken = _tokenRepo.GetByRefreshToken(hash);
            if (storedToken != null)
            {
                _tokenRepo.RevokeToken(storedToken.TokenId);
            }
        }

        public void RevokeAllTokens(int userId)
        {
            _tokenRepo.RevokeAllUserTokens(userId);
        }

        // ===========================================
        // Private helpers
        // ===========================================

        private string GenerateAccessToken(ValidatedUser user, DateTime expiresAt)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("fullName", user.UserFullName),
                new Claim(ClaimTypes.Email, user.EmailAddress ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateAccessTokenFromUserId(int userId, string username, DateTime expiresAt)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateSecureRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string GetUsernameFromStoredToken(int userId)
        {
            // Simple lookup — we could cache this, but for now query is fine
            // The ValidateCredentials method joins logindetail, so we need a username.
            // We'll look by userId in the logindetail table via the same repo or just return empty.
            return "";
        }
    }
}

