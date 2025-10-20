using RemTech.Core.Shared.Result;

namespace Identity.Domain.Roles.ValueObjects;

public sealed record RoleName
{
    public const int MaxLength = 100;
    public string Value { get; }

    public static RoleName User = new("USER");

    public static RoleName Admin = new("ADMIN");

    public static RoleName Root = new("ROOT");

    private RoleName(string value) => Value = value;

    public static Status<RoleName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("Роль не может быть пустой.");

        if (name.Length > MaxLength)
            return Error.Validation($"Роль превышает длину {MaxLength} символов.");

        return new RoleName(name);
    }
}
