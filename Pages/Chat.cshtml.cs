using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FraudVerificationDemo.Models;
using FraudVerificationDemo.Modules.Chat.Models;
using FraudVerificationDemo.Modules.Chat.Services;
using System.Text.Json;

namespace FraudVerificationDemo.Pages;

[Authorize]
public class ChatModel : PageModel
{
    private readonly ChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ChatModel> _logger;

    public ChatModel(
        ChatService chatService,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment environment,
        ILogger<ChatModel> logger)
    {
        _chatService = chatService;
        _userManager = userManager;
        _environment = environment;
        _logger = logger;
    }

    public List<ChatConversation> Conversations { get; set; } = new();
    public ChatConversation? CurrentConversation { get; set; }
    public ConversationStats Stats { get; set; } = new();

    public async Task OnGetAsync(Guid? id = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return;

        // Load all conversations
        Conversations = await _chatService.GetUserConversationsAsync(user.Id);

        // Load specific conversation if ID provided
        if (id.HasValue)
        {
            CurrentConversation = await _chatService.GetConversationAsync(id.Value, user.Id);
        }

        // Get stats
        Stats = await _chatService.GetUserStatsAsync(user.Id);
    }

    /// <summary>
    /// API endpoint: Get conversations list (for HTMX or AJAX)
    /// </summary>
    public async Task<IActionResult> OnGetConversationsAsync(string? search = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        List<ChatConversation> conversations;

        if (!string.IsNullOrWhiteSpace(search))
        {
            conversations = await _chatService.SearchConversationsAsync(user.Id, search);
        }
        else
        {
            conversations = await _chatService.GetUserConversationsAsync(user.Id);
        }

        return new JsonResult(new
        {
            success = true,
            conversations = conversations.Select(c => new
            {
                id = c.Id,
                title = c.Title,
                time = c.UpdatedAt.ToString("HH:mm"),
                messageCount = c.Messages.Count
            })
        });
    }

    /// <summary>
    /// API endpoint: Get messages for a conversation
    /// </summary>
    public async Task<IActionResult> OnGetMessagesAsync(Guid conversationId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var conversation = await _chatService.GetConversationAsync(conversationId, user.Id);
        if (conversation == null) return NotFound();

        var messages = conversation.Messages.Select(m => new
        {
            id = m.Id,
            role = m.Role.ToString().ToLower(),
            content = m.Content,
            time = m.CreatedAt.ToString("HH:mm"),
            attachments = m.Attachments.Select(a => new
            {
                id = a.Id,
                fileName = a.FileName,
                contentType = a.ContentType,
                fileSize = a.FileSize,
                url = $"/uploads/chat/{a.FilePath}"
            }).ToList(),
            analysis = m.Analysis != null ? new
            {
                riskLevel = m.Analysis.RiskLevel,
                advice = m.Analysis.Advice,
                detectedPatterns = m.Analysis.DetectedPatterns
            } : null
        });

        return new JsonResult(new
        {
            success = true,
            conversation = new
            {
                id = conversation.Id,
                title = conversation.Title
            },
            messages
        });
    }

    /// <summary>
    /// API endpoint: Create new conversation
    /// </summary>
    public async Task<IActionResult> OnPostNewConversationAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var conversation = await _chatService.CreateConversationAsync(user.Id);

        return new JsonResult(new
        {
            success = true,
            conversationId = conversation.Id
        });
    }

    /// <summary>
    /// API endpoint: Send message with optional file uploads
    /// </summary>
    public async Task<IActionResult> OnPostSendMessageAsync(
        [FromForm] Guid? conversationId,
        [FromForm] string? message,
        [FromForm] List<IFormFile>? files)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        try
        {
            // Create new conversation if none exists
            Guid convId;
            if (!conversationId.HasValue || conversationId.Value == Guid.Empty)
            {
                var newConv = await _chatService.CreateConversationAsync(user.Id);
                convId = newConv.Id;
            }
            else
            {
                convId = conversationId.Value;
            }

            // Handle file uploads
            var attachments = new List<ChatAttachment>();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Length > 10 * 1024 * 1024) // 10MB limit
                    {
                        return BadRequest(new { success = false, error = $"{file.FileName} exceeds the 10 MB file size limit." });
                    }

                    var attachment = await SaveFileAsync(file);
                    attachments.Add(attachment);
                }
            }

            // Validate content
            if (string.IsNullOrWhiteSpace(message) && attachments.Count == 0)
            {
                return BadRequest(new { success = false, error = "Enter a message or upload a file." });
            }

            // Add user message
            var userMessage = await _chatService.AddUserMessageAsync(
                convId,
                message ?? string.Empty,
                attachments);

            // Generate AI response
            var assistantMessage = await _chatService.GenerateAssistantResponseAsync(convId, userMessage.Id);

            return new JsonResult(new
            {
                success = true,
                conversationId = convId,
                userMessage = new
                {
                    id = userMessage.Id,
                    role = "user",
                    content = userMessage.Content,
                    time = userMessage.CreatedAt.ToString("HH:mm"),
                    attachments = userMessage.Attachments.Select(a => new
                    {
                        id = a.Id,
                        fileName = a.FileName,
                        contentType = a.ContentType,
                        fileSize = a.FileSize,
                        url = $"/uploads/chat/{a.FilePath}"
                    }).ToList()
                },
                assistantMessage = new
                {
                    id = assistantMessage.Id,
                    role = "assistant",
                    content = assistantMessage.Content,
                    time = assistantMessage.CreatedAt.ToString("HH:mm"),
                    analysis = assistantMessage.Analysis != null ? new
                    {
                        riskLevel = assistantMessage.Analysis.RiskLevel,
                        advice = assistantMessage.Analysis.Advice,
                        detectedPatterns = assistantMessage.Analysis.DetectedPatterns
                    } : null
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, new { success = false, error = "Something went wrong while sending your message." });
        }
    }

    /// <summary>
    /// API endpoint: Delete conversation
    /// </summary>
    public async Task<IActionResult> OnPostDeleteConversationAsync([FromBody] JsonElement data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!data.TryGetProperty("conversationId", out var idElement))
        {
            return BadRequest(new { success = false, error = "Missing conversationId" });
        }

        if (!Guid.TryParse(idElement.GetString(), out var conversationId))
        {
            return BadRequest(new { success = false, error = "Invalid conversationId" });
        }

        var result = await _chatService.DeleteConversationAsync(conversationId, user.Id);

        return new JsonResult(new { success = result });
    }

    /// <summary>
    /// API endpoint: Clear all conversations for current user
    /// </summary>
    public async Task<IActionResult> OnPostClearAllConversationsAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var conversations = await _chatService.GetUserConversationsAsync(user.Id);
        foreach (var conv in conversations)
        {
            await _chatService.DeleteConversationAsync(conv.Id, user.Id);
        }

        return new JsonResult(new { success = true, count = conversations.Count });
    }

    private async Task<ChatAttachment> SaveFileAsync(IFormFile file)
    {
        // Create upload directory if it doesn't exist
        var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "chat");
        Directory.CreateDirectory(uploadDir);

        // Generate unique filename
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadDir, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return new ChatAttachment
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FilePath = uniqueFileName
        };
    }
}
