using OpenMind.CRM.Application.Dtos;

namespace OpenMind.CRM.Application.Services.Interfaces;

public interface IEmailService
{
    string ProviderName { get; }
    Task<List<EmailDto>> GetEmailsAsync(int userId, int maxResults = 10, string? pageToken = null);
    Task<EmailDto?> GetEmailByIdAsync(int userId, string emailId);
    Task<bool> MarkEmailAsReadAsync(int userId, string emailId, bool isRead = true);
}