using System.Text.RegularExpressions;

namespace Users.Module.CommonAbstractions;

internal sealed class EmailValidation
{
    private static readonly Regex EmailValidityRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled
    );

    public void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new EmailEmptyException();
        if (!EmailValidityRegex.IsMatch(email))
            throw new InvalidEmailFormatException();
        if (email.StartsWith(".") || email.EndsWith("."))
            throw new InvalidEmailFormatException();
        if (email.Contains(".."))
            throw new InvalidEmailFormatException();
        string localPart = email.Split('@')[0];
        if (localPart.Length > 64)
            throw new InvalidEmailFormatException();
        if (email.Length > 254)
            throw new InvalidEmailFormatException();
    }
}
