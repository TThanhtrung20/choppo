using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Models;

/// <summary>
/// User-defined protection principle set when calm, before any incident.
/// </summary>
public class ProtectionRule
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Rule must be between 10 and 500 characters.")]
    [Display(Name = "Protection Rule")]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ApplicationUser? User { get; set; }
}
