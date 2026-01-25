using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

public sealed record PermissionDescription
{
	private PermissionDescription(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<PermissionDescription> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<PermissionDescription>)Error.Validation("Описание разрешения не может быть пустым.")
			: (Result<PermissionDescription>)new PermissionDescription(value);
	}
}
