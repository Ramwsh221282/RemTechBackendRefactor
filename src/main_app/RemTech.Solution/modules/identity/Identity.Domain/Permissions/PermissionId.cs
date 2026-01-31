using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Permissions;

/// <summary>
/// Идентификатор разрешения.
/// </summary>
public readonly record struct PermissionId
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="PermissionId"/> с новым уникальным значением.
	/// </summary>
	public PermissionId()
	{
		Value = Guid.NewGuid();
	}

	private PermissionId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение идентификатора разрешения.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создает новый уникальный идентификатор разрешения.
	/// </summary>
	/// <returns>Новый уникальный идентификатор разрешения.</returns>
	public static PermissionId New()
	{
		return new(Guid.NewGuid());
	}

	/// <summary>
	/// Создает экземпляр <see cref="PermissionId"/> с валидацией.
	/// </summary>
	/// <param name="value">Значение идентификатора разрешения.</param>
	/// <returns>Результат создания экземпляра <see cref="PermissionId"/>.</returns>
	public static Result<PermissionId> Create(Guid value)
	{
		return value == Guid.Empty
			? Error.Validation("Идентификатор разрешения не может быть пустым.")
			: new PermissionId(value);
	}
}
