using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

public sealed record PermissionName
{
	private const int MaxLength = 256;

	private PermissionName(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<PermissionName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Название разрешения не может быть пустым.");
		return value.Length > MaxLength
			? (Result<PermissionName>)Error.Validation("Название разрешения не может быть длиннее 128 символов.")
			: (Result<PermissionName>)new PermissionName(value);
	}
}
