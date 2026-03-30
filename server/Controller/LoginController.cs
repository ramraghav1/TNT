using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Models;
using Business.Services;
using static Domain.Models.Auth;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [Route("/login")]
    [HttpPost]
    public IActionResult Post([FromBody] AuthLoginRequest request)
    {
        try
        {
            var result = _loginService.Login(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred during login" });
        }
    }

    [Route("refresh")]
    [HttpPost]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = _loginService.RefreshToken(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { success = false, message = "Invalid or expired refresh token" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "An error occurred during token refresh" });
        }
    }

    [Authorize]
    [Route("logout")]
    [HttpPost]
    public IActionResult Logout([FromBody] LogoutRequest request)
    {
        try
        {
            _loginService.Logout(request);
            return Ok(new { success = true, message = "Logged out successfully" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "An error occurred during logout" });
        }
    }

    [Authorize]
    [Route("logout-all")]
    [HttpPost]
    public IActionResult LogoutAll()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { success = false, message = "Invalid token" });
            }
            _loginService.LogoutAll(userId);
            return Ok(new { success = true, message = "Logged out from all devices" });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "An error occurred" });
        }
    }
}
