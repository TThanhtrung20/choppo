using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Modules.Chat.Models;

/// <summary>
/// Stores the AI analysis result for scam detection
/// </summary>
public sealed class ScamAnalysisResult
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MessageId { get; set; }

    public ChatMessage? Message { get; set; }

    /// <summary>
    /// Risk level: Low, Medium, High
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string RiskLevel { get; set; } = "Low";

    /// <summary>
    /// Confidence score 0-100
    /// </summary>
    public int ConfidenceScore { get; set; }

    /// <summary>
    /// Detected scam patterns/keywords
    /// </summary>
    [MaxLength(500)]
    public string? DetectedPatterns { get; set; }

    /// <summary>
    /// AI-generated advice for user
    /// </summary>
    [MaxLength(2000)]
    public string? Advice { get; set; }

    /// <summary>
    /// Full analysis text
    /// </summary>
    [MaxLength(4000)]
    public string? AnalysisText { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
