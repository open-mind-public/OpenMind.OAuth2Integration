using Microsoft.EntityFrameworkCore;
using OpenMind.CRM.Application.Services.Interfaces;
using OpenMind.CRM.Domain.Entities;
using OpenMind.CRM.Infrastructure.Data;

namespace OpenMind.CRM.Infrastructure.Repositories;

public class UserRepository(CrmDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
            .Include(u => u.OAuthTokens)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await context.Users
            .Include(u => u.OAuthTokens)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<OAuthToken?> GetOAuthTokenAsync(int userId, string provider)
    {
        return await context.OAuthTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Provider == provider);
    }

    public async Task<OAuthToken> SaveOAuthTokenAsync(OAuthToken token)
    {
        context.OAuthTokens.Add(token);
        await context.SaveChangesAsync();
        return token;
    }

    public async Task<OAuthToken> UpdateOAuthTokenAsync(OAuthToken token)
    {
        token.UpdatedAt = DateTime.UtcNow;
        context.OAuthTokens.Update(token);
        await context.SaveChangesAsync();
        return token;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
            return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteOAuthTokenAsync(int userId, string provider)
    {
        var token = await GetOAuthTokenAsync(userId, provider);
        if (token != null)
        {
            context.OAuthTokens.Remove(token);
            await context.SaveChangesAsync();
        }
    }
}