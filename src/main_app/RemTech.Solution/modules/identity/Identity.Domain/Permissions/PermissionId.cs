using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

public readonly record struct PermissionId
{
	public Guid Value { get; }

	public PermissionId()
	{
		Value = Guid.NewGuid();
	}

	private PermissionId(Guid value)
	{
		Value = value;
	}

	public static PermissionId New() => new(Guid.NewGuid());

	public static Result<PermissionId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор разрешения не может быть пустым.")
			: new PermissionId(value);
	}
}
