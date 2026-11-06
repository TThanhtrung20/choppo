using Microsoft.EntityFrameworkCore;
using FraudVerificationDemo.Data;
using FraudVerificationDemo.Modules.Chat.Models;

namespace FraudVerificationDemo.Modules.Chat.Services;

/// <summary>
/// Service for managing chat conversations and messages
/// </summary>
public class ChatService
{
    private readonly ApplicationDbContext _context;
    private readonly ScamAnalysisService _analysisService;

    public ChatService(ApplicationDbContext context, ScamAnalysisService analysisService)
    {
        _context = context;
        _analysisService = analysisService;
    }

    /// <summary>
    /// Get all conversations for a user, ordered by most recent
    /// </summary>
    public async Task<List<ChatConversation>> GetUserConversationsAsync(string userId)
    {
        var conversations = await _context.ChatConversations
            .Where(c => c.UserId == userId && c.Status == ChatConversationStatus.Active)
            .Include(c => c.Messages)
            .ToListAsync();

        // Order in memory to avoid SQLite DateTimeOffset issue
        return conversations
            .OrderByDescending(c => c.UpdatedAt)
            .ToList();
    }

    /// <summary>
    /// Get a specific conversation with all messages and attachments
    /// </summary>
    public async Task<ChatConversation?> GetConversationAsync(Guid conversationId, string userId)
    {
        var conversation = await _context.ChatConversations
            .Where(c => c.Id == conversationId && c.UserId == userId)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Attachments)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Analysis)
            .FirstOrDefaultAsync();

        // Order messages in memory to avoid SQLite DateTimeOffset issue
        if (conversation != null && conversation.Messages != null)
        {
            conversation.Messages = conversation.Messages.OrderBy(m => m.CreatedAt).ToList();
        }

        return conversation;
    }

    /// <summary>
    /// Create a new conversation
    /// </summary>
    public async Task<ChatConversation> CreateConversationAsync(string userId, string? title = null)
    {
        var conversation = new ChatConversation
        {
            UserId = userId,
            Title = title ?? "New conversation",
            Status = ChatConversationStatus.Active
        };

        _context.ChatConversations.Add(conversation);
        await _context.SaveChangesAsync();

        return conversation;
    }

    /// <summary>
    /// Add a user message to a conversation
    /// </summary>
    public async Task<ChatMessage> AddUserMessageAsync(
        Guid conversationId,
        string content,
        List<ChatAttachment>? attachments = null)
    {
        var conversation = await _context.ChatConversations
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation == null)
        {
            throw new InvalidOperationException("Conversation not found");
        }

        var message = new ChatMessage
        {
            ConversationId = conversationId,
            Role = ChatMessageRole.User,
            Content = content,
            Attachments = attachments ?? new List<ChatAttachment>()
        };

        _context.ChatMessages.Add(message);

        // Update conversation title if this is the first message
        if (!await _context.ChatMessages.AnyAsync(m => m.ConversationId == conversationId))
        {
            var title = !string.IsNullOrWhiteSpace(content)
                ? content.Trim().Replace("\n", " ").Substring(0, Math.Min(50, content.Length))
                : attachments?.Any() == true
                    ? $"[Attachment] {attachments[0].FileName}"
                    : "New conversation";

            conversation.Title = title;
        }

        conversation.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return message;
    }

    /// <summary>
    /// Generate AI assistant response with scam analysis
    /// </summary>
    public async Task<ChatMessage> GenerateAssistantResponseAsync(Guid conversationId, Guid userMessageId)
    {
        var userMessage = await _context.ChatMessages
            .Include(m => m.Attachments)
            .FirstOrDefaultAsync(m => m.Id == userMessageId);

        if (userMessage == null)
        {
            throw new InvalidOperationException("User message not found");
        }

        // Analyze content for scam patterns
        var analysis = _analysisService.AnalyzeContent(
            userMessage.Content,
            userMessage.Attachments.Count > 0);

        // Create assistant response message
        var responseMessage = new ChatMessage
        {
            ConversationId = conversationId,
            Role = ChatMessageRole.Assistant,
            Content = analysis.AnalysisText ?? "Đã phân tích nội dung của bạn.",
            Analysis = analysis
        };

        _context.ChatMessages.Add(responseMessage);

        // Update conversation timestamp
        var conversation = await _context.ChatConversations
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        if (conversation != null)
        {
            conversation.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync();

        return responseMessage;
    }

    /// <summary>
    /// Delete a conversation (soft delete by changing status)
    /// </summary>
    public async Task<bool> DeleteConversationAsync(Guid conversationId, string userId)
    {
        var conversation = await _context.ChatConversations
            .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

        if (conversation == null)
        {
            return false;
        }

        conversation.Status = ChatConversationStatus.Deleted;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Search conversations by title or content
    /// </summary>
    public async Task<List<ChatConversation>> SearchConversationsAsync(string userId, string query)
    {
        query = query.ToLower().Trim();

        var conversations = await _context.ChatConversations
            .Where(c => c.UserId == userId
                && c.Status == ChatConversationStatus.Active
                && c.Title.ToLower().Contains(query))
            .ToListAsync();

        // Order in memory to avoid SQLite DateTimeOffset issue
        return conversations
            .OrderByDescending(c => c.UpdatedAt)
            .Take(20)
            .ToList();
    }

    /// <summary>
    /// Get conversation statistics for a user
    /// </summary>
    public async Task<ConversationStats> GetUserStatsAsync(string userId)
    {
        var conversations = await _context.ChatConversations
            .Where(c => c.UserId == userId && c.Status == ChatConversationStatus.Active)
            .ToListAsync();

        var totalMessages = await _context.ChatMessages
            .Where(m => conversations.Select(c => c.Id).Contains(m.ConversationId))
            .CountAsync();

        var highRiskCount = await _context.ScamAnalysisResults
            .Where(a => a.RiskLevel == "High"
                && conversations.Select(c => c.Id).Contains(a.Message!.ConversationId))
            .CountAsync();

        return new ConversationStats
        {
            TotalConversations = conversations.Count,
            TotalMessages = totalMessages,
            HighRiskDetections = highRiskCount
        };
    }
}

public class ConversationStats
{
    public int TotalConversations { get; set; }
    public int TotalMessages { get; set; }
    public int HighRiskDetections { get; set; }
}
