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
    public void OnGet()
    {
        // No database queries, no authentication required
        // Content is injected in the view via SafetyContentService
    }
}
