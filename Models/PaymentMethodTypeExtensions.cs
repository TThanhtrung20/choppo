namespace FraudVerificationDemo.Models;

/// <summary>
/// Display helpers so payment method labels and icons stay consistent across pages.
/// </summary>
public static class PaymentMethodTypeExtensions
{
    public static string Label(this PaymentMethodType method) => method switch
    {
        PaymentMethodType.BankTransfer => "Bank transfer",
        PaymentMethodType.CreditCard => "Credit card",
        PaymentMethodType.DebitCard => "Debit card",
        PaymentMethodType.PaymentApp => "Payment app",
        PaymentMethodType.Check => "Check",
        _ => "Other"
    };

    /// <summary>Bootstrap Icons class name for the method.</summary>
    public static string Icon(this PaymentMethodType method) => method switch
    {
        PaymentMethodType.BankTransfer => "bi-bank",
        PaymentMethodType.CreditCard => "bi-credit-card",
        PaymentMethodType.DebitCard => "bi-credit-card-2-front",
        PaymentMethodType.PaymentApp => "bi-phone",
        PaymentMethodType.Check => "bi-pencil-square",
        _ => "bi-three-dots"
    };
}
