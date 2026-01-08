namespace OpenMind.CRM.Application.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(int userId, string email);
    int? GetUserIdFromToken(string token);
    bool ValidateToken(string token);
}