using System.Text.RegularExpressions;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.ValueObjects;

public sealed class UserEmail
{
    private static readonly Regex EmailValidityRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled
    );

    public const int MaxLength = 256;
    public const int LocalPathLength = 64;
    public string Email { get; }

    private UserEmail(string email) => Email = email;

    public static Status<UserEmail> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Error.Validation("Почта пользователя была пустой.");
        if (!EmailValidityRegex.IsMatch(email))
            return Error.Validation("Почта пользователя некорректного формата");
        if (email.StartsWith(".") || email.EndsWith("."))
            return Error.Validation("Почта пользователя некорректного формата");
        if (email.Contains(".."))
            return Error.Validation("Почта пользователя некорректного формата");
        string localPart = email.Split('@')[0];
        if (localPart.Length > 64)
            return Error.Validation("Почта пользователя некорректного формата");
        if (email.Length > 254)
            return Error.Validation("Почта пользователя некорректного формата");
        return new UserEmail(email);
    }
}
