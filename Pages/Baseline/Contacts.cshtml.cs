using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FraudVerificationDemo.Pages.Baseline;

[Authorize]
public class ContactsModel : PageModel
{
    public void OnGet()
    {
    }
}
