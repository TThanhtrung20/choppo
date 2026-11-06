using System.Text.RegularExpressions;
using FraudVerificationDemo.Modules.Chat.Models;

namespace FraudVerificationDemo.Modules.Chat.Services;

/// <summary>
/// Service for analyzing content for scam/fraud patterns.
/// Currently uses pattern matching. Can be extended with OpenAI later.
/// </summary>
public class ScamAnalysisService
{
    private static readonly List<ScamPattern> _patterns = new()
    {
        new ScamPattern
        {
            Pattern = @"otp|mã xác minh|mã pin|mật khẩu|password|verification code|one[- ]time code|passcode",
            Category = "Credential Theft",
            RiskWeight = 30,
            Description = "Requests sensitive information"
        },
        new ScamPattern
        {
            Pattern = @"chuyển tiền|tài khoản tạm giữ|phí ship|ứng trước|nạp tiền|hoàn tiền|đặt cọc|deposit|transfer money|wire transfer|pay(?:ment)? fee|upfront payment|send money",
            Category = "Financial Fraud",
            RiskWeight = 35,
            Description = "Requests a money transfer or fee payment"
        },
        new ScamPattern
        {
            Pattern = @"bấm vào|nhấp vào|đường link|liên kết|truy cập ngay|click here|http://|link lạ|click the link|open this link|visit this link|http://|https://",
            Category = "Phishing",
            RiskWeight = 25,
            Description = "Contains a suspicious link"
        },
        new ScamPattern
        {
            Pattern = @"trúng thưởng|hoa hồng cao|hoàn vốn|tri ân|việc nhẹ lương cao|nhiệm vụ|prize|winning|you(?:'|’)ve won|congratulations.{0,30}(?:win|won|prize)|high[- ]paying job|easy money|earn \$|commission",
            Category = "Prize/Job Scam",
            RiskWeight = 30,
            Description = "Impersonates a prize or easy-job offer"
        },
        new ScamPattern
        {
            Pattern = @"công an|viện kiểm sát|lệnh bắt|bộ công an|điều tra|tòa án|police|investigation|arrest warrant|law enforcement|prosecutor(?:'|’)s office|government agency|under investigation|linked to a case",
            Category = "Impersonation",
            RiskWeight = 40,
            Description = "Impersonates a government authority"
        },
        new ScamPattern
        {
            Pattern = @"khóa tài khoản|block account|suspend|ngừng hoạt động|bị khóa|will be locked|account.{0,20}(?:suspend|close|lock)|locked within|account access will end",
            Category = "Urgency Tactics",
            RiskWeight = 25,
            Description = "Creates a false sense of urgency"
        },
        new ScamPattern
        {
            Pattern = @"trong 24h|trong vòng|hết hạn|expire|urgent|gấp|ngay lập tức|immediately|within \d+ hours?|act now|final notice|immediate action|required today",
            Category = "Time Pressure",
            RiskWeight = 20,
            Description = "Uses time pressure"
        }
    };

    /// <summary>
    /// Analyze text content for scam patterns
    /// </summary>
    public ScamAnalysisResult AnalyzeContent(string content, bool hasAttachments = false)
    {
        if (string.IsNullOrWhiteSpace(content) && !hasAttachments)
        {
            return new ScamAnalysisResult
            {
                RiskLevel = "Low",
                ConfidenceScore = 0,
                AnalysisText = "There is no content to analyze.",
                Advice = "Provide the message content or describe the situation for a review."
            };
        }

        // If only attachments, return generic response
        if (string.IsNullOrWhiteSpace(content) && hasAttachments)
        {
            return new ScamAnalysisResult
            {
                RiskLevel = "Low",
                ConfidenceScore = 10,
                AnalysisText = "Attachment received. Describe its contents or the situation so Safeline AI can review it further.",
                Advice = "Describe what is shown in the image or explain how you received this file."
            };
        }

        var detectedPatterns = new List<ScamPattern>();
        int totalRiskScore = 0;

        // Check each pattern
        foreach (var pattern in _patterns)
        {
            if (Regex.IsMatch(content, pattern.Pattern, RegexOptions.IgnoreCase))
            {
                detectedPatterns.Add(pattern);
                totalRiskScore += pattern.RiskWeight;
            }
        }

        // Check for question context
        bool isQuestionAboutScam = Regex.IsMatch(content,
            @"có phải|lừa đảo|nghi ngờ|tin được không|thật không|an toàn không|scam|fraud|suspicious|is this legit|is this a scam|is it safe|should i trust|does this look",
            RegexOptions.IgnoreCase);

        // Adjust risk based on number of patterns
        if (detectedPatterns.Count >= 2 || (detectedPatterns.Count >= 1 && isQuestionAboutScam))
        {
            totalRiskScore += 20; // Bonus for multiple patterns
        }

        // Determine risk level
        string riskLevel;
        string analysisText;
        string advice;

        if (totalRiskScore >= 70 || detectedPatterns.Count >= 3)
        {
            riskLevel = "High";
            analysisText = GenerateHighRiskAnalysis(detectedPatterns, hasAttachments);
            advice = GenerateHighRiskAdvice();
        }
        else if (totalRiskScore >= 35 || detectedPatterns.Count >= 2)
        {
            riskLevel = "Medium";
            analysisText = GenerateMediumRiskAnalysis(detectedPatterns, hasAttachments);
            advice = GenerateMediumRiskAdvice();
        }
        else
        {
            riskLevel = "Low";
            analysisText = GenerateLowRiskAnalysis(detectedPatterns, hasAttachments);
            advice = GenerateLowRiskAdvice();
        }

        return new ScamAnalysisResult
        {
            RiskLevel = riskLevel,
            ConfidenceScore = Math.Min(totalRiskScore, 100),
            DetectedPatterns = string.Join(", ", detectedPatterns.Select(p => p.Category)),
            AnalysisText = analysisText,
            Advice = advice
        };
    }

    private string GenerateHighRiskAnalysis(List<ScamPattern> patterns, bool hasAttachments)
    {
        var categories = string.Join(", ", patterns.Select(p => p.Description).Distinct());
        var fileNote = hasAttachments ? "and the attached evidence " : "";

        return $"⚠️ Based on the content {fileNote}you provided, <strong>this may be a scam</strong>. " +
               $"This message or call contains common warning signs: {categories}. " +
               "These tactics are often used to steal money or personal information.";
    }

    private string GenerateMediumRiskAnalysis(List<ScamPattern> patterns, bool hasAttachments)
    {
        var categories = string.Join(", ", patterns.Select(p => p.Description).Distinct());
        var fileNote = hasAttachments ? "and the attached file " : "";

        return $"⚠️ This content {fileNote}contains <strong>some suspicious indicators</strong> that warrant caution. " +
               $"The following patterns were detected: {categories}. " +
               "This is not a definitive conclusion, so verify the details carefully before taking action.";
    }

    private string GenerateLowRiskAnalysis(List<ScamPattern> patterns, bool hasAttachments)
    {
        var fileNote = hasAttachments ? "and the attached file " : "";

        if (patterns.Count > 0)
        {
            return $"Some sensitive keywords were found in this content {fileNote}, " +
                   "but there is not enough evidence to determine that it is a scam. Stay cautious with requests involving money or personal information.";
        }

        return $"No clear warning signs were found among common indicators in this content {fileNote}. " +
               "Still, remain cautious with any unexpected or suspicious requests.";
    }

    private string GenerateHighRiskAdvice()
    {
        return "🛡️ <strong>Urgent safety steps:</strong><br/>" +
               "• Do <strong>NOT</strong> click any links<br/>" +
               "• Do <strong>NOT</strong> transfer money or pay fees<br/>" +
               "• Do <strong>NOT</strong> share one-time codes, passwords, or account details<br/>" +
               "• Do <strong>NOT</strong> install apps at the caller's request<br/>" +
               "• Verify directly with the bank or organization using its official contact details<br/>" +
               "• If you have already sent money, contact your bank and local authorities immediately";
    }

    private string GenerateMediumRiskAdvice()
    {
        return "⚠️ <strong>Recommended precautions:</strong><br/>" +
               "• Verify the message or caller through official channels<br/>" +
               "• Do not rush to act, especially when asked to transfer money<br/>" +
               "• Check phone numbers and email addresses against official contact details<br/>" +
               "• Research the organization or sender before trusting the request<br/>" +
               "• If you are unsure, ask someone you trust or a qualified professional";
    }

    private string GenerateLowRiskAdvice()
    {
        return "ℹ️ <strong>General safety tips:</strong><br/>" +
               "• Be cautious of unexpected requests to send money<br/>" +
               "• Never share one-time codes or passwords<br/>" +
               "• Check links carefully before opening them<br/>" +
               "• Verify information through official channels if you have concerns<br/>" +
               "• If anything seems unusual, pause and investigate before acting";
    }

    private class ScamPattern
    {
        public string Pattern { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int RiskWeight { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
