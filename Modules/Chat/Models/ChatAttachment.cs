using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Modules.Chat.Models;

/// <summary>
/// Represents a file attachment in a chat message (image, PDF, audio, video, etc.)
/// </summary>
public sealed class ChatAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MessageId { get; set; }

    public ChatMessage? Message { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Relative path to file in wwwroot/uploads/chat/
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
