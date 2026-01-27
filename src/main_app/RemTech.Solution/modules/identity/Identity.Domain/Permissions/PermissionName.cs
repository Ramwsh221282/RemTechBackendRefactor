using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

/// <summary>
/// Название разрешения.
/// </summary>
public sealed record PermissionName
{
	private const int MAX_LENGTH = 256;

	private PermissionName(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение названия разрешения.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает экземпляр <see cref="PermissionName"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение названия разрешения.</param>
	/// <returns>Результат создания экземпляра <see cref="PermissionName"/>.</returns>
	public static Result<PermissionName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Название разрешения не может быть пустым.");
		return value.Length > MAX_LENGTH
			? Error.Validation("Название разрешения не может быть длиннее 128 символов.")
			: new PermissionName(value);
	}
}
