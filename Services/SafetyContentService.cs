namespace FraudVerificationDemo.Services;

/// <summary>
/// Service that provides safety content for emergency flows.
/// Content is loaded from approved copy in CONTENT-LIBRARY.md.
/// All content is static and does not depend on AI, database, or Credits.
/// </summary>
public class SafetyContentService
{
    /// <summary>
    /// Highest-level reminder shown immediately on all 3 emergency entry points.
    /// Status: DRAFT - awaiting user approval.
    /// </summary>
    public string GetHighestLevelReminder()
    {
        return "If they will not give you time to verify, stop doing what they ask.\n\n" +
               "End the conversation or leave the request. Verify through a channel you already have.";
    }

    /// <summary>
    /// Independent verification guidance text.
    /// </summary>
    public string GetVerificationGuidance()
    {
        return "To verify independently:\n\n" +
               "✓ Use the number on your physical card or old bills – NOT a number they just gave you\n" +
               "✓ Use official contacts you found yourself (type the bank website yourself, don't click their link)\n" +
               "✓ Use a number you saved before today\n\n" +
               "Never use:\n" +
               "✗ A phone number they just told you\n" +
               "✗ A link or website they sent you\n" +
               "✗ An app they asked you to download";
    }

    /// <summary>
    /// Stop-action message for post-incident flows.
    /// </summary>
    public string GetStopActionMessage()
    {
        return "STOP - Do not send more money or information.\n" +
               "Do not respond to them again.";
    }

    /// <summary>
    /// Post-payment guidance by payment method.
    /// </summary>
    public string GetPostPaymentGuidance(string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "card" or "debit" or "credit" => GetCardGuidance(),
            "transfer" or "wire" or "bank" => GetTransferGuidance(),
            "giftcard" or "gift" => GetGiftCardGuidance(),
            "crypto" or "cryptocurrency" => GetCryptoGuidance(),
            "cash" or "mail" => GetCashGuidance(),
            "moneytransfer" or "westernunion" or "moneygram" => GetMoneyTransferGuidance(),
            _ => GetGenericPostPaymentGuidance()
        };
    }

    private string GetCardGuidance()
    {
        return "What to do now:\n\n" +
               "1. Lock or freeze your card in your bank's app\n" +
               "2. Call the fraud line on the back of your card\n" +
               "3. Tell them you gave your card information to someone you now believe was trying to scam you\n\n" +
               "Keep your card and any receipts.";
    }

    private string GetTransferGuidance()
    {
        return "What to do now:\n\n" +
               "1. Contact your bank or payment app immediately\n" +
               "2. Ask if the transfer can be stopped or reversed\n" +
               "3. Report this as a suspected scam\n\n" +
               "Use a number from your old statement or card, not a number the other person gave you.";
    }

    private string GetMoneyTransferGuidance()
    {
        return "What to do now:\n\n" +
               "1. Contact the money transfer company using their official number (look it up yourself)\n" +
               "2. Ask if the transfer can be stopped\n" +
               "3. Report this transaction as suspicious\n\n" +
               "Keep all receipts and transaction numbers.";
    }

    private string GetGiftCardGuidance()
    {
        return "What to do now:\n\n" +
               "1. Contact the gift card company (the brand on the card)\n" +
               "2. Report the card number as used in a scam\n" +
               "3. Ask if there's any way to freeze or recover the funds\n\n" +
               "Keep the physical card and receipt. Recovery is unlikely but report it anyway.";
    }

    private string GetCryptoGuidance()
    {
        return "What to do now:\n\n" +
               "1. Contact the crypto exchange or service you used\n" +
               "2. Report the transaction immediately\n" +
               "3. Save all transaction records, wallet addresses, and screenshots\n\n" +
               "Note: Crypto transactions usually cannot be reversed, but report it for their fraud tracking.";
    }

    private string GetCashGuidance()
    {
        return "What to do now:\n\n" +
               "1. If you just sent it: Contact the postal or delivery service immediately\n" +
               "2. Ask if the package can be stopped or returned\n" +
               "3. Save your tracking number and receipt\n\n" +
               "Act quickly – once delivered, cash cannot be recovered.";
    }

    private string GetGenericPostPaymentGuidance()
    {
        return "What to do now:\n\n" +
               "1. Contact the payment provider or your bank immediately\n" +
               "2. Report this as a suspected scam\n" +
               "3. Ask what options you have\n\n" +
               "Use an official contact number you find yourself, not one they gave you.";
    }

    /// <summary>
    /// Post-disclosure guidance by information type.
    /// </summary>
    public string GetPostDisclosureGuidance(string infoType)
    {
        return infoType.ToLower() switch
        {
            "password" => GetPasswordGuidance(),
            "otp" or "code" or "verification" => GetOtpGuidance(),
            "card" or "cardnumber" or "cvv" => GetCardNumberGuidance(),
            "ssn" or "social" or "id" => GetSsnGuidance(),
            "app" or "device" or "access" or "screenshare" => GetDeviceAccessGuidance(),
            _ => GetGenericPostDisclosureGuidance()
        };
    }

    private string GetPasswordGuidance()
    {
        return "What to do now:\n\n" +
               "1. Change your password immediately through the official app or website (type the address yourself)\n" +
               "2. Change passwords anywhere else you used the same password\n" +
               "3. Turn on two-factor authentication if available\n\n" +
               "Do NOT use a link they sent you to change your password.";
    }

    private string GetOtpGuidance()
    {
        return "What to do now:\n\n" +
               "1. Check for any new logins or payments in your official app\n" +
               "2. Change your password immediately\n" +
               "3. Turn on additional security settings if available\n" +
               "4. Contact the service to report unauthorized access\n\n" +
               "The code they got may have let them into your account.";
    }

    private string GetCardNumberGuidance()
    {
        return "What to do now:\n\n" +
               "1. Lock or freeze your card in your bank app immediately\n" +
               "2. Call the number on your card or statement\n" +
               "3. Ask about replacing your card or protecting your account\n\n" +
               "They now have enough information to make charges.";
    }

    private string GetSsnGuidance()
    {
        return "What to do now:\n\n" +
               "1. In the US: Place a credit freeze with all three credit bureaus (Equifax, Experian, TransUnion)\n" +
               "2. Visit IdentityTheft.gov for a recovery plan\n" +
               "3. Monitor your bank and credit accounts closely\n\n" +
               "This is serious. You may need identity theft protection services.";
    }

    private string GetDeviceAccessGuidance()
    {
        return "What to do now:\n\n" +
               "1. Turn off internet/WiFi on that device immediately\n" +
               "2. Do NOT open banking or email apps on that device\n" +
               "3. Use a different device to change your passwords\n" +
               "4. Get technical help to check for remote access apps\n\n" +
               "They may have control of your device right now. Use a different device to secure your accounts.";
    }

    private string GetGenericPostDisclosureGuidance()
    {
        return "What to do now:\n\n" +
               "1. Contact the relevant service or company immediately\n" +
               "2. Tell them what information was shared\n" +
               "3. Follow their guidance to secure your account\n\n" +
               "Use an official contact number or website you find yourself.";
    }

    /// <summary>
    /// Demo data warning for account/baseline pages.
    /// </summary>
    public string GetDemoDataWarning()
    {
        return "⚠️ DEMO DATA ONLY\n\n" +
               "This is a local demonstration. Do not enter:\n" +
               "• Real passwords or security codes\n" +
               "• Real full credit card or account numbers\n" +
               "• Real contact information\n" +
               "• Real details of any actual incident\n\n" +
               "Use clearly fictional information only.";
    }

    /// <summary>
    /// Disclaimer for footer.
    /// </summary>
    public string GetDisclaimer()
    {
        return "This system helps you pause and verify. It does not verify whether someone is trustworthy, " +
               "does not guarantee any transaction is safe, and does not take action on your behalf. " +
               "All decisions are yours. This is a demonstration for internal review only.";
    }
}
