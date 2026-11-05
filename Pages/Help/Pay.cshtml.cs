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
    public void OnGet()
    {
        // No database queries, no authentication required
        // Content is injected in the view via SafetyContentService
    }
}
