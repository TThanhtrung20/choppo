using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FraudVerificationDemo.Data;
using FraudVerificationDemo.Models;

namespace FraudVerificationDemo.Pages.Baseline;

[Authorize]
public class RulesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public RulesModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<ProtectionRule> Rules { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Rule must be between 10 and 500 characters.")]
        [Display(Name = "Protection Rule")]
        public string Content { get; set; } = default!;
    }

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        Rules = await _context.ProtectionRules
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var userId = _userManager.GetUserId(User);

        if (Input.Id.HasValue)
        {
            // Edit existing rule
            var rule = await _context.ProtectionRules
                .FirstOrDefaultAsync(r => r.Id == Input.Id && r.UserId == userId);

            if (rule == null)
            {
                StatusMessage = "Error: Rule not found.";
                return RedirectToPage();
            }

            rule.Content = Input.Content;
            rule.UpdatedAt = DateTime.UtcNow;

            StatusMessage = "Rule updated successfully.";
        }
        else
        {
            // Create new rule
            var rule = new ProtectionRule
            {
                UserId = userId!,
                Content = Input.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProtectionRules.Add(rule);
            StatusMessage = "Rule added successfully.";
        }

        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = _userManager.GetUserId(User);
        var rule = await _context.ProtectionRules
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (rule == null)
        {
            StatusMessage = "Error: Rule not found.";
            return RedirectToPage();
        }

        _context.ProtectionRules.Remove(rule);
        await _context.SaveChangesAsync();

        StatusMessage = "Rule deleted successfully.";
        return RedirectToPage();
    }
}
