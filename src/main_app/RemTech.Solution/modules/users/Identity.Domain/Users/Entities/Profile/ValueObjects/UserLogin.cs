using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Profile.ValueObjects;

public sealed record UserLogin
{
    public const int MaxLength = 100;
    public string Name { get; }

    private UserLogin(string name) => Name = name;

    public static Status<UserLogin> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Логин пользователя не может быть пустым.");

        if (name.Length > MaxLength)
            return Error.Validation($"Логин пользователя превышает длину: {MaxLength} символов.");

        return new UserLogin(name);
    }
}
