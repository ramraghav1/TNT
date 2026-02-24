using System;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces;
using static Repository.DataModels.AuthTokenDTO;

namespace Repository.Repositories
{
    public class AuthTokenRepository : IAuthTokenRepository
    {
        private readonly IDbConnection _dbConnection;

        public AuthTokenRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Look up user by username, returning password hash for BCrypt verification.
        /// Joins logindetail → userinformation.
        /// </summary>
        public ValidatedUserResponse? ValidateCredentials(string username)
        {
            string sql = @"
                SELECT
                    ld.loginid,
                    ld.userid,
                    ld.username,
                    ld.password AS passwordhash,
                    COALESCE(ui.userfullname, '') AS userfullname,
                    ui.emailaddress,
                    ui.mobilenumber
                FROM logindetail ld
                LEFT JOIN userinformation ui ON ui.userid = ld.userid
                WHERE LOWER(ld.username) = LOWER(@Username)
                LIMIT 1;";

            return _dbConnection.QuerySingleOrDefault<ValidatedUserResponse>(sql, new { Username = username });
        }

        /// <summary>
        /// Store a new refresh token + access token pair in the authtoken table.
        /// </summary>
        public void StoreToken(CreateAuthTokenRequest request)
        {
            string sql = @"
                INSERT INTO authtoken
                    (userid, refreshtoken, accesstoken, issuedat, expiresat, revoked, createdat, updatedat)
                VALUES
                    (@UserId, @RefreshToken, @AccessToken, NOW(), @ExpiresAt, false, NOW(), NOW());";

            _dbConnection.Execute(sql, request);
        }

        /// <summary>
        /// Find a valid (non-revoked, non-expired) token by the hashed refresh token value.
        /// </summary>
        public AuthTokenResponse? GetByRefreshToken(string refreshTokenHash)
        {
            string sql = @"
                SELECT tokenid, userid, refreshtoken, accesstoken,
                       issuedat, expiresat, revoked, revokedat,
                       ipaddress, useragent
                FROM authtoken
                WHERE refreshtoken = @Hash
                  AND revoked = false
                  AND expiresat > NOW()
                LIMIT 1;";

            return _dbConnection.QuerySingleOrDefault<AuthTokenResponse>(sql, new { Hash = refreshTokenHash });
        }

        /// <summary>
        /// Revoke a single token by setting revoked = true and revokedat = now.
        /// </summary>
        public void RevokeToken(int tokenId)
        {
            string sql = @"
                UPDATE authtoken
                SET revoked = true,
                    revokedat = NOW(),
                    updatedat = NOW()
                WHERE tokenid = @TokenId;";

            _dbConnection.Execute(sql, new { TokenId = tokenId });
        }

        /// <summary>
        /// Revoke ALL active tokens for a user (force logout from all devices).
        /// </summary>
        public void RevokeAllUserTokens(int userId)
        {
            string sql = @"
                UPDATE authtoken
                SET revoked = true,
                    revokedat = NOW(),
                    updatedat = NOW()
                WHERE userid = @UserId
                  AND revoked = false;";

            _dbConnection.Execute(sql, new { UserId = userId });
        }
    }
}
