using EasyShop.API.DTOs;
using EasyShop.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterDto registerDto)
    {
        var response = await _authService.RegisterAsync(registerDto);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);

        return Ok(response);
    }
}