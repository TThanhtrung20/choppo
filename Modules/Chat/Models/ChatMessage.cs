using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Modules.Chat.Models;

/// <summary>
/// One user or assistant message in a conversation. Provider metadata is
/// intentionally not included until an AI integration is chosen.
/// </summary>
public sealed class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ConversationId { get; set; }

    public ChatConversation? Conversation { get; set; }

    public ChatMessageRole Role { get; set; }

    [Required]
    [MaxLength(8000)]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public List<ChatAttachment> Attachments { get; set; } = [];

    public ScamAnalysisResult? Analysis { get; set; }
}
