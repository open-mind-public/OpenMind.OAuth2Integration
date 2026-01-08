using OpenMind.CRM.Application.Services.Interfaces;
using OpenMind.CRM.Domain.Entities;
using Microsoft.Extensions.Logging;
using OpenMind.CRM.Application.Dtos;

namespace OpenMind.CRM.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IPasswordService passwordService,
    ILogger<AuthService> logger)
    : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = jwtTokenService.GenerateToken(user.Id, user.Email);
        var expiresAt = DateTime.UtcNow.AddHours(24);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = MapToUserDto(user)
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var passwordHash = passwordService.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await userRepository.CreateAsync(user);

        return MapToUserDto(createdUser);
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var userId = jwtTokenService.GetUserIdFromToken(token);
            return userId.HasValue && await userRepository.GetByIdAsync(userId.Value) != null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating token");
            return false;
        }
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            HasGoogleAccess = user.OAuthTokens?.Any(t => t.Provider == "Google" && !string.IsNullOrEmpty(t.AccessToken)) ?? false,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}