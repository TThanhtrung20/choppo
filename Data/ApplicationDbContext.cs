using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FraudVerificationDemo.Models;
using FraudVerificationDemo.Modules.Chat.Models;

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

    // Chat module
    public DbSet<ChatConversation> ChatConversations { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatAttachment> ChatAttachments { get; set; }
    public DbSet<ScamAnalysisResult> ScamAnalysisResults { get; set; }

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

        // Chat module relationships
        builder.Entity<ChatConversation>()
            .HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChatMessage>()
            .HasMany(m => m.Attachments)
            .WithOne(a => a.Message)
            .HasForeignKey(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ChatMessage>()
            .HasOne(m => m.Analysis)
            .WithOne(a => a.Message)
            .HasForeignKey<ScamAnalysisResult>(a => a.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chat indexes
        builder.Entity<ChatConversation>()
            .HasIndex(c => c.UserId);

        builder.Entity<ChatConversation>()
            .HasIndex(c => c.CreatedAt);

        builder.Entity<ChatMessage>()
            .HasIndex(m => m.ConversationId);

        builder.Entity<ChatAttachment>()
            .HasIndex(a => a.MessageId);

        builder.Entity<ScamAnalysisResult>()
            .HasIndex(a => a.MessageId);
    }
}
