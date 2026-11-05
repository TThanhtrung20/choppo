using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FraudVerificationDemo.Models;

namespace FraudVerificationDemo.Data;

/// <summary>
/// Database context for the fraud verification demo.
/// Uses SQLite for local-only demo data storage.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProtectionRule> ProtectionRules { get; set; }
    public DbSet<TrustedContact> TrustedContacts { get; set; }
    public DbSet<UsualPaymentMethod> UsualPaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure relationships - cascade delete when user is deleted
        builder.Entity<ProtectionRule>()
            .HasOne(pr => pr.User)
            .WithMany(u => u.ProtectionRules)
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<TrustedContact>()
            .HasOne(tc => tc.User)
            .WithMany(u => u.TrustedContacts)
            .HasForeignKey(tc => tc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UsualPaymentMethod>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.UsualPaymentMethods)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for better query performance
        builder.Entity<ProtectionRule>()
            .HasIndex(pr => pr.UserId);

        builder.Entity<TrustedContact>()
            .HasIndex(tc => tc.UserId);

        builder.Entity<UsualPaymentMethod>()
            .HasIndex(pm => pm.UserId);
    }
}
