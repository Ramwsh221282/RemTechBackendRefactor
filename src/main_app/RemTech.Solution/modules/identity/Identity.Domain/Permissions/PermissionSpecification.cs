namespace Identity.Domain.Permissions;

/// <summary>
/// Спецификация для поиска разрешений.
/// </summary>
public sealed class PermissionSpecification
{
	/// <summary>
	/// Идентификатор разрешения.
	/// </summary>
	public Guid? Id { get; private set; }

	/// <summary>
	/// Название разрешения.
	/// </summary>
	public string? Name { get; private set; }

	/// <summary>
	/// Описание разрешения.
	/// </summary>
	public string? Description { get; private set; }

	/// <summary>
	/// Требуется ли блокировка разрешения.
	/// </summary>
	public bool LockRequired { get; private set; }

	/// <summary>
	/// Устанавливает идентификатор разрешения.
	/// </summary>
	/// <param name="id">Идентификатор разрешения.</param>
	/// <returns>Текущий экземпляр спецификации разрешения.</returns>
	public PermissionSpecification WithId(Guid id)
	{
		if (Id.HasValue)
		{
			return this;
		}

		Id = id;
		return this;
	}

	/// <summary>
	/// Устанавливает название разрешения.
	/// </summary>
	/// <param name="name">Название разрешения.</param>
	/// <returns>Текущий экземпляр спецификации разрешения.</returns>
	public PermissionSpecification WithName(string name)
	{
		if (!string.IsNullOrWhiteSpace(Name))
		{
			return this;
		}

		Name = name;
		return this;
	}

	/// <summary>
	/// Устанавливает описание разрешения.
	/// </summary>
	/// <param name="description">Описание разрешения.</param>
	/// <returns>Текущий экземпляр спецификации разрешения.</returns>
	public PermissionSpecification WithDescription(string description)
	{
		if (!string.IsNullOrWhiteSpace(Description))
		{
			return this;
		}

		Description = description;
		return this;
	}

	/// <summary>
	/// Устанавливает требование блокировки разрешения.
	/// </summary>
	/// <returns>Текущий экземпляр спецификации разрешения.</returns>
	public PermissionSpecification WithLock()
	{
		LockRequired = true;
		return this;
	}
}
