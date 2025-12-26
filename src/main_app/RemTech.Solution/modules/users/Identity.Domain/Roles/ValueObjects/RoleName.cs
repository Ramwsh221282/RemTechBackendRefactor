using Identity.Contracts;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Roles.ValueObjects;

public sealed record RoleName
{
    public const int MaxLength = 100;
    public string Value { get; }

    public static readonly RoleName User = new(DefaultRoleNames.User);

    public static readonly RoleName Admin = new(DefaultRoleNames.Admin);

    public static readonly RoleName Root = new(DefaultRoleNames.Root);

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
