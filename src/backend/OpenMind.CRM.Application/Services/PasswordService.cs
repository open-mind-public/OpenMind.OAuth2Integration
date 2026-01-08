using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenMind.CRM.Application.Services.Interfaces;

namespace OpenMind.CRM.Application.Services;

public class PasswordService(IConfiguration configuration) : IPasswordService
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + configuration["JWT:Secret"]));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var computedHash = HashPassword(password);
        return computedHash == hash;
    }
}
