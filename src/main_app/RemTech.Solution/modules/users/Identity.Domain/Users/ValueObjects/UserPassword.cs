using Identity.Domain.Users.Ports;
using Identity.Domain.Users.Ports.Passwords;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.ValueObjects;

public sealed record UserPassword
{
    private const int MinLength = 8;
    public string Password { get; }

    public UserPassword(string password) => Password = password;

    public static Status<UserPassword> Create(string password)
    {
        if (string.IsNullOrEmpty(password))
            return Error.Validation("Пароль пользователя был пустым.");
        if (password.Length < MinLength)
            return Error.Validation($"Пароль пользователя менее: {MinLength} символов.");
        if (!password.Any(char.IsUpper))
            return Error.Validation("Пароль пользователя должен содержать заглавную букву.");
        if (!password.Any(char.IsLower))
            return Error.Validation("Пароль пользователя должен содержать строчную букву.");
        if (!password.Any(char.IsDigit))
            return Error.Validation("Пароль пользователя должен содержать числовой символ.");
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            return Error.Validation("Пароль пользователя должен содержать спец. символ");
        return new UserPassword(password);
    }

    public Status Verification(IPasswordManager manager, HashedUserPassword hashed)
    {
        return !manager.Verify(Password, hashed.Password)
            ? Status.Failure(new Error("Пароль не совпадает.", ErrorCodes.Unauthorized))
            : Status.Success();
    }
}
