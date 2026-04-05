using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Repository.Repositories;
using Bussiness.Services;
using static Repository.DataModels.UserManagementDTO;

namespace Bussiness.Services
{
    public interface IUserManagementService
    {
        CreateUserResponse CreateUser(CreateUserRequest request, string baseUrl);
        List<UserListDTO> GetUsersByTenant(long tenantId);
        List<UserListDTO> GetUsersByOrganization(long orgId);
        void DeactivateUser(int userId);
        void ReactivateUser(int userId);
        PasswordResetResponse RequestPasswordReset(string email, string baseUrl);
        bool ResetPassword(string token, string newPassword);
    }

    public class UserManagementService : IUserManagementService
    {
        private readonly IUserManagementRepository _userRepo;
        private readonly IPasswordResetRepository _passwordResetRepo;
        private readonly IEmailService _emailService;

        public UserManagementService(
            IUserManagementRepository userRepo,
            IPasswordResetRepository passwordResetRepo,
            IEmailService emailService)
        {
            _userRepo = userRepo;
            _passwordResetRepo = passwordResetRepo;
            _emailService = emailService;
        }

        public CreateUserResponse CreateUser(CreateUserRequest request, string baseUrl)
        {
            // Generate username from email
            string username = request.EmailAddress;
            
            // Generate random password
            string generatedPassword = GenerateRandomPassword(12);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(generatedPassword);

            // Create user
            int userId = _userRepo.CreateUser(
                request.UserFullName,
                request.EmailAddress,
                request.MobileNumber,
                request.Address,
                request.TenantId,
                request.OrgId,
                request.BranchId,
                request.CreatedBy
            );

            // Create login detail
            int loginId = _userRepo.CreateLoginDetail(
                userId,
                username,
                passwordHash,
                request.TenantId,
                request.OrgId
            );

            // Assign roles if provided
            if (request.RoleIds != null && request.RoleIds.Any())
            {
                _userRepo.AssignRolesToUser(userId, request.RoleIds, request.CreatedBy);
            }

            // Create password reset token for user to set their own password
            string resetToken = GenerateSecureToken();
            DateTime expiresAt = DateTime.UtcNow.AddDays(7); // Give 7 days to set password
            
            _passwordResetRepo.CreateResetToken(userId, request.EmailAddress, resetToken, expiresAt);

            // Generate reset link
            string resetLink = $"{baseUrl}/reset-password?token={resetToken}";

            // Send welcome email with credentials
            bool emailSent = _emailService.SendWelcomeEmailAsync(
                request.EmailAddress,
                request.UserFullName,
                username,
                generatedPassword,
                resetLink
            ).Result;

            return new CreateUserResponse
            {
                UserId = userId,
                LoginId = loginId,
                Username = username,
                GeneratedPassword = generatedPassword,
                EmailAddress = request.EmailAddress,
                EmailSent = emailSent
            };
        }

        public List<UserListDTO> GetUsersByTenant(long tenantId)
        {
            return _userRepo.GetUsersByTenant(tenantId);
        }

        public List<UserListDTO> GetUsersByOrganization(long orgId)
        {
            return _userRepo.GetUsersByOrganization(orgId);
        }

        public void DeactivateUser(int userId)
        {
            _userRepo.UpdateUserStatus(userId, false);
        }

        public void ReactivateUser(int userId)
        {
            _userRepo.UpdateUserStatus(userId, true);
        }

        public PasswordResetResponse RequestPasswordReset(string email, string baseUrl)
        {
            // TODO: Get user by email from repository (need to add this method)
            // For now, assuming we have userId
            
            // Generate secure token
            string token = GenerateSecureToken();
            DateTime expiresAt = DateTime.UtcNow.AddHours(24);

            // TODO: Get userId from email lookup
            // long tokenId = _passwordResetRepo.CreateResetToken(userId, email, token, expiresAt);

            string resetLink = $"{baseUrl}/reset-password?token={token}";

            // Send email
            // bool emailSent = _emailService.SendPasswordResetEmailAsync(email, "User", resetLink).Result;

            return new PasswordResetResponse
            {
                Token = token,
                ResetLink = resetLink,
                EmailSent = true // emailSent
            };
        }

        public bool ResetPassword(string token, string newPassword)
        {
            // Get valid token
            var tokenInfo = _passwordResetRepo.GetValidToken(token);
            if (tokenInfo == null)
            {
                return false; // Invalid or expired token
            }

            // Hash new password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // TODO: Update password in logindetail table (need to add this method to repository)
            // For now, just mark token as used
            _passwordResetRepo.MarkTokenAsUsed(tokenInfo.Id);
            
            // Invalidate all other tokens for this user
            _passwordResetRepo.InvalidateUserTokens(tokenInfo.UserId);

            // Send notification email
            _emailService.SendPasswordChangedNotificationAsync(tokenInfo.Email, "User").Wait();

            return true;
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => validChars[random.Next(validChars.Length)])
                .ToArray());
        }

        private string GenerateSecureToken()
        {
            byte[] tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
