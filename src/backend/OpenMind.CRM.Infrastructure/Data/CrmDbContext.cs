using Microsoft.EntityFrameworkCore;
using OpenMind.CRM.Domain.Entities;

namespace OpenMind.CRM.Infrastructure.Data;

public class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<OAuthToken> OAuthTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(255);
        });

        // OAuthToken configuration
        modelBuilder.Entity<OAuthToken>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Provider).IsRequired().HasMaxLength(50);
            entity.Property(t => t.AccessToken).IsRequired().HasMaxLength(1000);
            entity.Property(t => t.Scopes).IsRequired().HasMaxLength(500);
            
            // Create unique index on UserId and Provider combination
            entity.HasIndex(t => new { t.UserId, t.Provider }).IsUnique();
            
            entity.HasOne(t => t.User)
                  .WithMany(u => u.OAuthTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}