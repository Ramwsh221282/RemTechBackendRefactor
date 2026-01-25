using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

public sealed record PermissionDescription
{
    public string Value { get; }

    private PermissionDescription(string value)
    {
        Value = value;
    }

    public static Result<PermissionDescription> Create(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? (Result<PermissionDescription>)Error.Validation("Описание разрешения не может быть пустым.")
            : (Result<PermissionDescription>)new PermissionDescription(value);
    }
}
