using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Интерфейс репозитория для управления токенами обновления.
/// </summary>
public interface IRefreshTokensRepository
{
	/// <summary>
	/// Проверяет существование токена обновления для указанного аккаунта.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом проверки существования токена обновления.</returns>
	Task<bool> Exists(Guid accountId, CancellationToken ct = default);

	/// <summary>
	/// Добавляет новый токен обновления в репозиторий.
	/// </summary>
	/// <param name="token">Токен обновления для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Add(RefreshToken token, CancellationToken ct = default);

	/// <summary>
	/// Обновляет существующий токен обновления в репозитории.
	/// </summary>
	/// <param name="token">Токен обновления для обновления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Update(RefreshToken token, CancellationToken ct = default);

	/// <summary>
	/// Находит токен обновления по идентификатору аккаунта.
	/// </summary>
	/// <param name="accountId">Идентификатор аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом поиска токена обновления.</returns>
	Task<Result<RefreshToken>> Find(Guid accountId, CancellationToken ct = default);

	/// <summary>
	/// Находит токен обновления по его строковому представлению.
	/// </summary>
	/// <param name="refreshToken">Строковое представление токена обновления.</param>
	/// <param name="withLock">Флаг, указывающий, следует ли блокировать запись при поиске.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом поиска токена обновления.</returns>
	Task<Result<RefreshToken>> Find(string refreshToken, bool withLock = false, CancellationToken ct = default);

	/// <summary>
	/// Удаляет токен обновления из репозитория.
	/// </summary>
	/// <param name="token">Токен обновления для удаления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Delete(RefreshToken token, CancellationToken ct = default);

	/// <summary>
	/// Удаляет токены обновления для указанного аккаунта.
	/// </summary>
	/// <param name="AccountId">Идентификатор аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Delete(Guid AccountId, CancellationToken ct = default);
}
