using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FraudVerificationDemo.Pages.Help
{
    [AllowAnonymous]
    public class SelectModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
