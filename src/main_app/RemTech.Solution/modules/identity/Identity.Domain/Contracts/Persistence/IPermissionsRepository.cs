using Identity.Domain.Permissions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Интерфейс репозитория для управления разрешениями.
/// </summary>
public interface IPermissionsRepository
{
	/// <summary>
	/// Проверяет существование разрешения по спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для проверки существования разрешения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом проверки существования разрешения.</returns>
	Task<bool> Exists(PermissionSpecification specification, CancellationToken ct = default);

	/// <summary>
	/// Добавляет новое разрешение в репозиторий.
	/// </summary>
	/// <param name="permission">Разрешение для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Add(Permission permission, CancellationToken ct = default);

	/// <summary>
	/// Добавляет коллекцию разрешений в репозиторий.
	/// </summary>
	/// <param name="permissions">Коллекция разрешений для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Add(IEnumerable<Permission> permissions, CancellationToken ct = default);

	/// <summary>
	/// Получает одно разрешение по спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для получения разрешения.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом получения разрешения.</returns>
	Task<Result<Permission>> GetSingle(PermissionSpecification specification, CancellationToken ct = default);

	/// <summary>
	/// Получает коллекцию разрешений по спецификациям.
	/// </summary>
	/// <param name="specifications">Коллекция спецификаций для получения разрешений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом получения коллекции разрешений.</returns>
	Task<IEnumerable<Permission>> GetMany(
		IEnumerable<PermissionSpecification> specifications,
		CancellationToken ct = default
	);
}
