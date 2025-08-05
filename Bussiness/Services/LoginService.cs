using System;
using Domain.Models;
using BCrypt.Net;
using Repository.Interfaces;
using AutoMapper;

namespace Business.Services
{
    

    public interface ILoginService
    {
        bool CheckUserValid(LoginRequest loginRequest);
        
    }

    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginRepository;
        private readonly IMapper _mapper;
        public LoginService(ILoginRepository loginRepository,IMapper mapper)
        {
            _loginRepository = loginRepository;
            _mapper = mapper;
        }

        public bool CheckUserValid(LoginRequest loginRequest)
        {
            // TODO: Retrieve hashed password from your database for this user
            string hashedPasswordFromDb = GetHashedPasswordFromDatabase(loginRequest);

            return ComparePassword(loginRequest.Password, hashedPasswordFromDb)
                   && !IsPasswordChangeNeeded(loginRequest);
        }

        private string GetHashedPasswordFromDatabase(LoginRequest loginRequest)
        {
            // Placeholder - Replace this with actual DB lookup logic
            return "dfdsfds"; // example hash, not valid!
        }

        private bool ComparePassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }

        private bool IsPasswordChangeNeeded(LoginRequest loginRequest)
        {
            // TODO: Implement password expiration or change logic here
            return false;
        }
        private LoggedInUserInformation GetLoggedInUserInformation(string userName)
        {
            var result = _loginRepository.GetLoginUserInformation(userName);

            if (result == null)
            {
                return null;
            }

            var loginUserInfo = _mapper.Map<LoggedInUserInformation>(result);
            return loginUserInfo;
        }
        

    }
}
