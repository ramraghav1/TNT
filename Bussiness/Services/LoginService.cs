using System;
using AutoMapper;
using Repository.Interfaces;
using static Domain.Models.Auth;

namespace Business.Services
{
    public interface ILoginService
    {
        /// <summary>
        /// Authenticate user credentials, record login attempt, and return tokens.
        /// </summary>
        AuthLoginResponse Login(AuthLoginRequest request);

        /// <summary>
        /// Refresh an access token using a valid refresh token (token rotation).
        /// </summary>
        RefreshTokenResponse RefreshToken(RefreshTokenRequest request);

        /// <summary>
        /// Logout by revoking the provided refresh token.
        /// </summary>
        void Logout(LogoutRequest request);

        /// <summary>
        /// Force logout from all devices by revoking all tokens for the user.
        /// </summary>
        void LogoutAll(int userId);
    }

    public class LoginService : ILoginService
    {
        private readonly IAuthTokenRepository _authTokenRepo;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public LoginService(
            IAuthTokenRepository authTokenRepo,
            ITokenService tokenService,
            IMapper mapper)
        {
            _authTokenRepo = authTokenRepo;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public AuthLoginResponse Login(AuthLoginRequest request)
        {
            // 1. Look up user by username
            var userRecord = _authTokenRepo.ValidateCredentials(request.Username);
            if (userRecord == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // 2. Verify BCrypt password
            bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, userRecord.PasswordHash);
            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // 3. Map to domain ValidatedUser and generate tokens
            var validatedUser = _mapper.Map<ValidatedUser>(userRecord);

            return _tokenService.GenerateTokens(validatedUser, request.IpAddress, request.UserAgent);
        }

        public RefreshTokenResponse RefreshToken(RefreshTokenRequest request)
        {
            return _tokenService.RefreshTokens(request.RefreshToken, request.IpAddress, request.UserAgent);
        }

        public void Logout(LogoutRequest request)
        {
            _tokenService.RevokeToken(request.RefreshToken);
        }

        public void LogoutAll(int userId)
        {
            _tokenService.RevokeAllTokens(userId);
        }

    }
}
