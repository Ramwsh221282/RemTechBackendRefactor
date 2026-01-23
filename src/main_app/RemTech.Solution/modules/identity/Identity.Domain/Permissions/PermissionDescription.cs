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
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Описание разрешения не может быть пустым.");
		return new PermissionDescription(value);
	}
}
