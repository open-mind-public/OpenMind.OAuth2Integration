using OpenMind.CRM.Domain.Entities;

namespace OpenMind.CRM.Application.Services.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<OAuthToken?> GetOAuthTokenAsync(int userId, string provider);
    Task<OAuthToken> SaveOAuthTokenAsync(OAuthToken token);
    Task<OAuthToken> UpdateOAuthTokenAsync(OAuthToken token);
    Task DeleteOAuthTokenAsync(int userId, string provider);
}