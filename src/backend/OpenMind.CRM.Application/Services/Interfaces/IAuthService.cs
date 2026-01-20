using OpenMind.CRM.Application.Dtos;

namespace OpenMind.CRM.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request);
    Task<UserDto> RegisterAsync(RegisterRequest request);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<bool> ValidateTokenAsync(string token);
}