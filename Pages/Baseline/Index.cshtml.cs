using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FraudVerificationDemo.Data;
using FraudVerificationDemo.Models;

namespace FraudVerificationDemo.Pages.Baseline;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public int ProtectionRulesCount { get; set; }
    public int TrustedContactsCount { get; set; }
    public int UsualPaymentMethodsCount { get; set; }
    
    public List<ProtectionRule> RecentRules { get; set; } = new();
    public List<TrustedContact> RecentContacts { get; set; } = new();
    public List<UsualPaymentMethod> RecentPaymentMethods { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);

        // Get counts
        ProtectionRulesCount = await _context.ProtectionRules
            .Where(r => r.UserId == userId)
            .CountAsync();

        TrustedContactsCount = await _context.TrustedContacts
            .Where(c => c.UserId == userId)
            .CountAsync();

        UsualPaymentMethodsCount = await _context.UsualPaymentMethods
            .Where(p => p.UserId == userId)
            .CountAsync();
            
        // Get recent items (top 3 each)
        RecentRules = await _context.ProtectionRules
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(3)
            .ToListAsync();
            
        RecentContacts = await _context.TrustedContacts
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(3)
            .ToListAsync();
            
        RecentPaymentMethods = await _context.UsualPaymentMethods
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();
    }
}
