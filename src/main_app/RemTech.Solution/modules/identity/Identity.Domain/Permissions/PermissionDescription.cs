using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

/// <summary>
/// Описание разрешения.
/// </summary>
public sealed record PermissionDescription
{
	private PermissionDescription(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение описания разрешения.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает экземпляр <see cref="PermissionDescription"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение описания разрешения.</param>
	/// <returns>Результат создания экземпляра <see cref="PermissionDescription"/>.</returns>
	public static Result<PermissionDescription> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<PermissionDescription>)Error.Validation("Описание разрешения не может быть пустым.")
			: (Result<PermissionDescription>)new PermissionDescription(value);
	}
}
