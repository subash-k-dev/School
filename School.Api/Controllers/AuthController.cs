using Microsoft.AspNetCore.Mvc;
using School.Application.DTOs.Auth;
using School.Application.Services;

namespace School.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var result = await _auth.LoginAsync(req);
        return result is null ? Unauthorized("Invalid username/password") : Ok(result);
    }
}