using System;
using Repository.Interfaces;

namespace Business.Services
{
    public class TokenService
    {
        //    private readonly ILoginRepository _loginRepository;
        //    private readonly IJwtGenerator _jwtGenerator;

        //    public TokenService(ILoginRepository loginRepository, IJwtGenerator jwtGenerator)
        //    {
        //        _loginRepository = loginRepository;
        //        _jwtGenerator = jwtGenerator;
        //    }

        //    public TokenResult RefreshToken(string refreshToken)
        //    {
        //        // 1. Validate the refresh token
        //        var storedToken = _tokenRepository.GetByTokenHash(Hash(refreshToken));
        //        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.Revoked)
        //            throw new UnauthorizedAccessException("Invalid or expired token");

        //        // 2. Revoke the old token
        //        storedToken.Revoked = true;
        //        storedToken.RevokedAt = DateTime.UtcNow;
        //        _tokenRepository.Update(storedToken);

        //        // 3. Generate new access & refresh tokens
        //        var newAccessToken = _jwtGenerator.GenerateAccessToken(storedToken.UserId);
        //        var newRefreshToken = GenerateSecureRefreshToken();

        //        // 4. Store new refresh token
        //        _tokenRepository.Add(new RefreshToken
        //        {
        //            UserId = storedToken.UserId,
        //            RefreshToken = Hash(newRefreshToken),
        //            IssuedAt = DateTime.UtcNow,
        //            ExpiresAt = DateTime.UtcNow.AddDays(7),
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow
        //        });

        //        // 5. Return result
        //        return new TokenResult
        //        {
        //            AccessToken = newAccessToken,
        //            RefreshToken = newRefreshToken
        //        };
        //    }

        //    private string GenerateSecureRefreshToken()
        //    {
        //        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        //    }

        //    private string Hash(string token)
        //    {
        //        // Use a secure hashing method, like SHA256 + salt or HMAC
        //        using var sha = System.Security.Cryptography.SHA256.Create();
        //        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        //        var hash = sha.ComputeHash(bytes);
        //        return Convert.ToBase64String(hash);
        //    }
        //}

        //public class TokenResult
        //{
        //    public string AccessToken { get; set; }
        //    public string RefreshToken { get; set; }
        //}
    }
}
