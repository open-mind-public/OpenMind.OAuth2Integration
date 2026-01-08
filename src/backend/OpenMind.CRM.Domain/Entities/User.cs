using System.ComponentModel.DataAnnotations;

namespace OpenMind.CRM.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;
    
    public string? PasswordHash { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<OAuthToken> OAuthTokens { get; set; } = new List<OAuthToken>();
}