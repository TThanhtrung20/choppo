using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Models;

/// <summary>
/// Person the user can contact when unsure what to do.
/// Phone numbers must use clearly fictional format (555-xxxx) for demo.
/// </summary>
public class TrustedContact
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^555-\d{4}$", ErrorMessage = "Demo numbers must use format: 555-0100")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Relationship")]
    public string? Relationship { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ApplicationUser? User { get; set; }
}
