namespace OpenMind.CRM.Application.Services.Interfaces;

public interface IOAuthService
{
    string ProviderName { get; }
    string GenerateAuthorizationUrl(int userId);
    Task<bool> HandleAuthorizationCallbackAsync(string code, string state);
    Task<bool> HasValidTokenAsync(int userId);
    Task<bool> RevokeTokenAsync(int userId);
}