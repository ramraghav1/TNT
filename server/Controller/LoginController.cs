using Microsoft.AspNetCore.Mvc;
using Domain.Models;
using Business.Services;
using static Domain.Models.Auth;

[ApiController]
[Route("api/[controller]")]
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
}
