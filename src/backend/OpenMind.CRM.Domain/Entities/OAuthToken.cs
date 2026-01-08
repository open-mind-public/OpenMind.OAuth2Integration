using System.ComponentModel.DataAnnotations;

namespace OpenMind.CRM.Domain.Entities;

public class OAuthToken
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = string.Empty; // e.g., "Google", "Microsoft", "LinkedIn"
    
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    public string? RefreshToken { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(500)]
    public string Scopes { get; set; } = string.Empty; // Comma-separated list of scopes
    
    // Navigation properties
    public User User { get; set; } = null!;
}