using EasyShop.API.DTOs;

namespace EasyShop.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
}