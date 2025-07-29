using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Models;
using System;
using Business.Services;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILoginService _loginService;

    public LoginController(IOptions<JwtSettings> jwtOptions,ILoginService loginService)
    {
        _jwtSettings = jwtOptions.Value;
        _loginService = loginService;
    }

    [HttpPost]
    public IActionResult Post([FromBody] LoginRequest request)
    {
        bool result = _loginService.CheckUserValid(request);
        // Simulate credential validation (replace with real logic)
        if (request.Username == "admin" && request.Password == "password")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }

        return Unauthorized("Invalid credentials");
    }
    
}
