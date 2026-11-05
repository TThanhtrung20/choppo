using System.ComponentModel.DataAnnotations;

namespace FraudVerificationDemo.Models;

/// <summary>
/// Record of how user normally pays bills or people they regularly send money to.
/// DISPLAY ONLY in MVP - no comparison logic.
/// </summary>
public class UsualPaymentMethod
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Recipient Name")]
    public string RecipientName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Method")]
    public PaymentMethodType Method { get; set; }

    [StringLength(50)]
    [Display(Name = "Usual Amount (optional)")]
    public string? UsualAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public ApplicationUser? User { get; set; }
}

public enum PaymentMethodType
{
    BankTransfer,
    CreditCard,
    DebitCard,
    PaymentApp,
    Check,
    Other
}
