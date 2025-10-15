using System.Text.RegularExpressions;
using RemTech.Result.Pattern;

namespace Mailing.Domain.CommonContext.ValueObjects;

public sealed record EmailAddress
{
    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static Result<EmailAddress> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Строка почтового адреса пустая.");

        if (!EmailRegex.IsMatch(value))
            return Error.Validation($"Строка адреса {value} некорректного формата.");

        return new EmailAddress(value);
    }
}
