using System;
using System.Data;
using Dapper;

namespace Repository.Repositories
{
    public interface IPasswordResetRepository
    {
        long CreateResetToken(int userId, string email, string token, DateTime expiresAt);
        PasswordResetTokenInfo? GetValidToken(string token);
        void MarkTokenAsUsed(long tokenId);
        void InvalidateUserTokens(int userId);
    }

    public class PasswordResetTokenInfo
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }

    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly IDbConnection _dbConnection;

        public PasswordResetRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public long CreateResetToken(int userId, string email, string token, DateTime expiresAt)
        {
            string sql = @"
                INSERT INTO password_reset_tokens (user_id, email, token, expires_at)
                VALUES (@UserId, @Email, @Token, @ExpiresAt)
                RETURNING id";

            return _dbConnection.QuerySingle<long>(sql, new 
            { 
                UserId = userId, 
                Email = email, 
                Token = token, 
                ExpiresAt = expiresAt 
            });
        }

        public PasswordResetTokenInfo? GetValidToken(string token)
        {
            string sql = @"
                SELECT id AS Id, user_id AS UserId, email AS Email, token AS Token, 
                       expires_at AS ExpiresAt, is_used AS IsUsed
                FROM password_reset_tokens
                WHERE token = @Token 
                  AND is_used = false 
                  AND expires_at > NOW()
                LIMIT 1";

            return _dbConnection.QuerySingleOrDefault<PasswordResetTokenInfo>(sql, new { Token = token });
        }

        public void MarkTokenAsUsed(long tokenId)
        {
            string sql = @"
                UPDATE password_reset_tokens
                SET is_used = true, used_at = NOW()
                WHERE id = @TokenId";

            _dbConnection.Execute(sql, new { TokenId = tokenId });
        }

        public void InvalidateUserTokens(int userId)
        {
            string sql = @"
                UPDATE password_reset_tokens
                SET is_used = true, used_at = NOW()
                WHERE user_id = @UserId AND is_used = false";

            _dbConnection.Execute(sql, new { UserId = userId });
        }
    }
}
