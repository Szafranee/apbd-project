using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_01.Interfaces;
using Project_01.RequestModels.Auth;

namespace Project_01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        var response = await authService.LoginAsync(request);
        if (response != null)
        {
            return Ok(response);
        }
        return Unauthorized("Invalid username or password");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
    {
        var response = await authService.RefreshTokenAsync(request.RefreshToken);
        if (response != null)
        {
            return Ok(response);
        }
        return Unauthorized("Invalid refresh token");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
    {
        if (await authService.RegisterAsync(request))
        {
            return Ok("Employee registered successfully");
        }
        return BadRequest("Login already exists");
    }
}