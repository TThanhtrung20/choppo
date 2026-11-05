using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FraudVerificationDemo.Pages.Baseline;

[Authorize]
public class PaymentsModel : PageModel
{
    public void OnGet()
    {
    }
}
