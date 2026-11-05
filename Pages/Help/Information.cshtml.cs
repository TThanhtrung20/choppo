using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FraudVerificationDemo.Pages.Help;

/// <summary>
/// Emergency page: Someone is asking for user's information or device access.
/// This page must work WITHOUT login and WITHOUT database access.
/// All content is static from SafetyContentService.
/// </summary>
[AllowAnonymous]
public class InformationModel : PageModel
{
    public string? ScenarioType { get; set; }
    public string ScenarioTitle { get; set; } = "Someone is asking for my information";
    
    public void OnGet(string? type)
    {
        ScenarioType = type;
        
        ScenarioTitle = type switch
        {
            "credentials" => "Login/Password Request",
            "financial" => "Financial Information Request",
            "personal" => "Personal Identity Request",
            "verification" => "Verification Code Request",
            "documents" => "Document Upload Request",
            _ => "Someone is asking for my information"
        };
    }
}
