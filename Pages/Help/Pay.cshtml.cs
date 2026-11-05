using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FraudVerificationDemo.Pages.Help;

/// <summary>
/// Emergency page: Someone is asking the user to pay.
/// This page must work WITHOUT login and WITHOUT database access.
/// All content is static from SafetyContentService.
/// </summary>
[AllowAnonymous]
public class PayModel : PageModel
{
    public string? ScenarioType { get; set; }
    public string ScenarioTitle { get; set; } = "Someone is asking me to pay";
    
    public void OnGet(string? type)
    {
        ScenarioType = type;
        
        ScenarioTitle = type switch
        {
            "unexpected" => "Unexpected Payment Request",
            "urgency" => "Urgent Payment Demand",
            "authority" => "Authority Figure Requesting Payment",
            "friend" => "Friend/Family Asking for Money",
            "prize" => "Prize or Refund (Pay Fees First)",
            "investment" => "Investment Opportunity",
            _ => "Someone is asking me to pay"
        };
    }
}
