using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Modules.Chat.Models;

/// <summary>
/// Represents one chat session. This is a domain model only; it is not yet
/// registered with Entity Framework or connected to an AI provider.
/// </summary>
public sealed class ChatConversation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Optional Identity user ID. Null supports anonymous/demo sessions later.
    /// </summary>
    [MaxLength(450)]
    public string? UserId { get; set; }

    [MaxLength(120)]
    public string Title { get; set; } = "New conversation";

    public ChatConversationStatus Status { get; set; } = ChatConversationStatus.Active;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<ChatMessage> Messages { get; set; } = [];
}
