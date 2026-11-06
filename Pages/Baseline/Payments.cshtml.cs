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
public class PaymentsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PaymentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<UsualPaymentMethod> PaymentMethods { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Recipient Name")]
        public string RecipientName { get; set; } = default!;

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethodType Method { get; set; }

        [StringLength(50)]
        [Display(Name = "Usual Amount (optional)")]
        public string? UsualAmount { get; set; }
    }

    /// <summary>True when the form is editing an existing item (?edit=id or a failed edit post).</summary>
    public bool IsEditing => Input?.Id is not null;

    public async Task<IActionResult> OnGetAsync(int? edit)
    {
        await LoadAsync();

        if (edit is int id)
        {
            var item = PaymentMethods.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                StatusMessage = "Error: Payment method not found.";
                return RedirectToPage();
            }

            Input = new InputModel { Id = item.Id, RecipientName = item.RecipientName, Method = item.Method, UsualAmount = item.UsualAmount };
        }

        return Page();
    }

    private async Task LoadAsync()
    {
        var userId = _userManager.GetUserId(User);
        PaymentMethods = await _context.UsualPaymentMethods
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.RecipientName)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var userId = _userManager.GetUserId(User);

        if (Input.Id.HasValue)
        {
            // Edit existing payment method
            var payment = await _context.UsualPaymentMethods
                .FirstOrDefaultAsync(p => p.Id == Input.Id && p.UserId == userId);

            if (payment == null)
            {
                StatusMessage = "Error: Payment method not found.";
                return RedirectToPage();
            }

            payment.RecipientName = Input.RecipientName;
            payment.Method = Input.Method;
            payment.UsualAmount = Input.UsualAmount;
            payment.UpdatedAt = DateTime.UtcNow;

            StatusMessage = "Payment method updated successfully.";
        }
        else
        {
            // Create new payment method
            var payment = new UsualPaymentMethod
            {
                UserId = userId!,
                RecipientName = Input.RecipientName,
                Method = Input.Method,
                UsualAmount = Input.UsualAmount,
                CreatedAt = DateTime.UtcNow
            };

            _context.UsualPaymentMethods.Add(payment);
            StatusMessage = "Payment method added successfully.";
        }

        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = _userManager.GetUserId(User);
        var payment = await _context.UsualPaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

        if (payment == null)
        {
            StatusMessage = "Error: Payment method not found.";
            return RedirectToPage();
        }

        _context.UsualPaymentMethods.Remove(payment);
        await _context.SaveChangesAsync();

        StatusMessage = "Payment method deleted successfully.";
        return RedirectToPage();
    }
}
