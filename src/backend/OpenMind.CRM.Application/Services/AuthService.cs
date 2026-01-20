using OpenMind.CRM.Application.Services.Interfaces;
using OpenMind.CRM.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using OpenMind.CRM.Application.Dtos;
using Google.Apis.Auth;

namespace OpenMind.CRM.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IPasswordService passwordService,
    IConfiguration configuration,
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

        // Check if user signed up with Google (no password set)
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new UnauthorizedAccessException("This account uses Google Sign-In. Please login with Google.");
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

    public async Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest request)
    {
        var googleClientId = configuration["Google:ClientId"] 
            ?? throw new InvalidOperationException("Google ClientId is not configured");

        GoogleJsonWebSignature.Payload payload;
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (InvalidJwtException ex)
        {
            logger.LogWarning(ex, "Invalid Google ID token");
            throw new UnauthorizedAccessException("Invalid Google token");
        }

        var user = await userRepository.GetByEmailAsync(payload.Email);
        
        if (user == null)
        {
            // Create new user from Google profile
            user = new User
            {
                Email = payload.Email,
                FirstName = payload.GivenName ?? payload.Name?.Split(' ').FirstOrDefault() ?? "User",
                LastName = payload.FamilyName ?? payload.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
                PasswordHash = null, // Google users don't have a password
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            user = await userRepository.CreateAsync(user);
            logger.LogInformation("Created new user from Google login: {Email}", payload.Email);
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