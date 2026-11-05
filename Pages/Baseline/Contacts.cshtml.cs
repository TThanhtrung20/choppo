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
public class ContactsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ContactsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<TrustedContact> Contacts { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    [TempData]
    public string? StatusMessage { get; set; }

    public class InputModel
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Name")]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^555-\d{4}$", ErrorMessage = "Demo numbers must use format: 555-0100")]
        public string Phone { get; set; } = default!;

        [StringLength(50)]
        [Display(Name = "Relationship")]
        public string? Relationship { get; set; }
    }

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);
        Contacts = await _context.TrustedContacts
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
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
            // Edit existing contact
            var contact = await _context.TrustedContacts
                .FirstOrDefaultAsync(c => c.Id == Input.Id && c.UserId == userId);

            if (contact == null)
            {
                StatusMessage = "Error: Contact not found.";
                return RedirectToPage();
            }

            contact.Name = Input.Name;
            contact.Phone = Input.Phone;
            contact.Relationship = Input.Relationship;
            contact.UpdatedAt = DateTime.UtcNow;

            StatusMessage = "Contact updated successfully.";
        }
        else
        {
            // Create new contact
            var contact = new TrustedContact
            {
                UserId = userId!,
                Name = Input.Name,
                Phone = Input.Phone,
                Relationship = Input.Relationship,
                CreatedAt = DateTime.UtcNow
            };

            _context.TrustedContacts.Add(contact);
            StatusMessage = "Contact added successfully.";
        }

        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userId = _userManager.GetUserId(User);
        var contact = await _context.TrustedContacts
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (contact == null)
        {
            StatusMessage = "Error: Contact not found.";
            return RedirectToPage();
        }

        _context.TrustedContacts.Remove(contact);
        await _context.SaveChangesAsync();

        StatusMessage = "Contact deleted successfully.";
        return RedirectToPage();
    }
}
