using System;
using Domain.Models;
using BCrypt.Net;

namespace Business.Services
{
    public interface ILoginService
    {
        bool CheckUserValid(LoginRequest loginRequest);
        
    }

    public class LoginService : ILoginService
    {
        public LoginService()
        {
            // Inject dependencies here (e.g., a user repository) if needed
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
    }
}
