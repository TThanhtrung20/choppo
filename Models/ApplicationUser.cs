using Microsoft.AspNetCore.Identity;

namespace FraudVerificationDemo.Models;

/// <summary>
/// Application user for local demo authentication.
/// Extends IdentityUser to support ASP.NET Core Identity.
/// </summary>
public class ApplicationUser : IdentityUser
{
    // Navigation properties for baseline data
    public ICollection<ProtectionRule> ProtectionRules { get; set; } = new List<ProtectionRule>();
    public ICollection<TrustedContact> TrustedContacts { get; set; } = new List<TrustedContact>();
    public ICollection<UsualPaymentMethod> UsualPaymentMethods { get; set; } = new List<UsualPaymentMethod>();
}
