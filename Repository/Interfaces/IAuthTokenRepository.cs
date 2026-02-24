using static Repository.DataModels.AuthTokenDTO;

namespace Repository.Interfaces
{
    public interface IAuthTokenRepository
    {
        /// <summary>
        /// Validate username/password and return user info. Returns null if invalid.
        /// </summary>
        ValidatedUserResponse? ValidateCredentials(string username);

        /// <summary>
        /// Store a new refresh token record.
        /// </summary>
        void StoreToken(CreateAuthTokenRequest request);

        /// <summary>
        /// Get a valid (non-revoked, non-expired) token by refresh token hash.
        /// </summary>
        AuthTokenResponse? GetByRefreshToken(string refreshTokenHash);

        /// <summary>
        /// Revoke a specific token (on refresh or logout).
        /// </summary>
        void RevokeToken(int tokenId);

        /// <summary>
        /// Revoke ALL tokens for a user (force logout everywhere).
        /// </summary>
        void RevokeAllUserTokens(int userId);
    }
}
