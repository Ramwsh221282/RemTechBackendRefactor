namespace Identity.Domain.Permissions;

/// <summary>
/// Представляет разрешение в системе.
/// </summary>
/// <param name="id">Идентификатор разрешения.</param>
/// <param name="name">Имя разрешения.</param>
/// <param name="description">Описание разрешения.</param>
public sealed class Permission(PermissionId id, PermissionName name, PermissionDescription description)
{
	private Permission(Permission permission)
		: this(permission.Id, permission.Name, permission.Description) { }

	/// <summary>
	/// Идентификатор разрешения.
	/// </summary>
	public PermissionId Id { get; } = id;

	/// <summary>
	/// Имя разрешения.
	/// </summary>
	public PermissionName Name { get; private set; } = name;

	/// <summary>
	/// Описание разрешения.
	/// </summary>
	public PermissionDescription Description { get; private set; } = description;

	/// <summary>
	/// Создает новое разрешение с заданным именем и описанием.
	/// </summary>
	/// <param name="name">Имя разрешения.</param>
	/// <param name="description">Описание разрешения.</param>
	/// <returns>Новое разрешение.</returns>
	public static Permission CreateNew(PermissionName name, PermissionDescription description)
	{
		PermissionId id = PermissionId.New();
		return new Permission(id, name, description);
	}

	/// <summary>
	/// Переименовывает разрешение.
	/// </summary>
	/// <param name="newName">Новое имя разрешения.</param>
	public void Rename(PermissionName newName)
	{
		Name = newName;
	}

	/// <summary>
	/// Изменяет описание разрешения.
	/// </summary>
	/// <param name="newDescription">Новое описание разрешения.</param>
	public void ChangeDescription(PermissionDescription newDescription)
	{
		Description = newDescription;
	}

	/// <summary>
	/// Создает копию текущего разрешения.
	/// </summary>
	/// <returns>Копия текущего разрешения.</returns>
	public Permission Clone()
	{
		return new(this);
	}
}
