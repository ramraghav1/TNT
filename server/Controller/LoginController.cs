using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Business.Services;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    //private readonly JwtSettings _jwtSettings;
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        //_jwtSettings = jwtOptions;
        _loginService = loginService;
    }
    [Route("/login")]
    [HttpPost]
    public IActionResult Post([FromBody] LoginRequest request)
    {

        return Ok(new { success = true, message = "Login successful" });

    }

    //[Route("/loginrequest")]
    //[HttpPost]
    //public IActionResult Login([FromBody] LoginRequest request)
    //{
    //    bool result = _loginService.CheckUserValid(request);
    //    if (request.Username == "admin" && request.Password == "password")
    //    {
    //        var claims = new[]
    //        {
    //            new Claim(ClaimTypes.Name, request.Username),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //        };

    //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
    //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //        var token = new JwtSecurityToken(
    //            issuer: _jwtSettings.Issuer,
    //            audience: _jwtSettings.Audience,
    //            claims: claims,
    //            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
    //            signingCredentials: creds
    //        );

    //        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    //        return Ok(new { token = tokenString });
    //    }

    //    return Unauthorized("Invalid credentials");
    //}

}
