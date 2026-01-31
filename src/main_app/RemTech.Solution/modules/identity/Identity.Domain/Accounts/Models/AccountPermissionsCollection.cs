using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

/// <summary>
/// Коллекция разрешений аккаунта.
/// </summary>
/// <param name="id">Идентификатор аккаунта.</param>
/// <param name="permissions">Список разрешений аккаунта.</param>
public sealed class AccountPermissionsCollection(AccountId id, IEnumerable<Permission> permissions)
{
	private readonly List<Permission> _permissions = [.. permissions];

	private AccountPermissionsCollection(AccountPermissionsCollection collection)
		: this(collection.AccountId, [.. collection._permissions]) { }

	/// <summary>
	/// Идентификатор аккаунта.
	/// </summary>
	public AccountId AccountId { get; } = id;

	/// <summary>
	/// Список разрешений аккаунта.
	/// </summary>
	public IReadOnlyList<Permission> Permissions => _permissions;

	/// <summary>
	/// Количество разрешений в коллекции.
	/// </summary>
	public int Count => _permissions.Count;

	/// <summary>
	/// Создает пустую коллекцию разрешений для указанного аккаунта.
	/// </summary>
	/// <param name="id">Идентификатор аккаунта.</param>
	/// <returns>Пустая коллекция разрешений.</returns>
	public static AccountPermissionsCollection Empty(AccountId id)
	{
		return new(id, []);
	}

	/// <summary>
	/// Добавляет разрешение в коллекцию.
	/// </summary>
	/// <param name="permission">Разрешение для добавления.</param>
	/// <returns>Результат операции добавления.</returns>
	public Result<Unit> Add(Permission permission)
	{
		if (HasPermission(permission))
		{
			return Error.Conflict("Разрешение уже присвоено учетной записи.");
		}

		_permissions.Add(permission);
		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Удаляет разрешение из коллекции.
	/// </summary>
	/// <param name="permission">Разрешение для удаления.</param>
	/// <returns>Результат операции удаления.</returns>
	public Result<Unit> Remove(Permission permission)
	{
		if (!HasPermission(permission))
		{
			return Error.NotFound("Разрешение не найдено.");
		}

		_permissions.Remove(permission);
		return Result.Success(Unit.Value);
	}

	/// <summary>
	/// Клонирует коллекцию разрешений.
	/// </summary>
	/// <returns>Клонированная коллекция разрешений.</returns>
	public AccountPermissionsCollection Clone()
	{
		return new(this);
	}

	/// <summary>
	/// Находит разрешение по идентификатору.
	/// </summary>
	/// <param name="permissionId">Идентификатор разрешения.</param>
	/// <returns>Результат операции поиска разрешения.</returns>
	public Result<Permission> Find(Guid permissionId)
	{
		Permission? permission = _permissions.FirstOrDefault(p => p.Id.Value == permissionId);
		return permission is null ? Error.NotFound("Разрешение не найдено.") : Result.Success(permission);
	}

	/// <summary>
	/// Находит разрешение по имени.
	/// </summary>
	/// <param name="permissionName">Имя разрешения.</param>
	/// <returns>Результат операции поиска разрешения.</returns>
	public Result<Permission> Find(string permissionName)
	{
		Permission? permission = _permissions.FirstOrDefault(p => p.Name.Value == permissionName);
		return permission is null ? Error.NotFound("Разрешение не найдено.") : Result.Success(permission);
	}

	/// <summary>
	/// Проверяет наличие разрешения в коллекции.
	/// </summary>
	/// <param name="permission">Разрешение для проверки.</param>
	/// <returns>True, если разрешение присутствует в коллекции; в противном случае false.</returns>
	private bool HasPermission(Permission permission)
	{
		return _permissions.Any(p => p.Id == permission.Id);
	}
}
