using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Интерфейс репозитория для управления аккаунтами.
/// </summary>
public interface IAccountsRepository
{
	/// <summary>
	/// Добавляет новый аккаунт в репозиторий.
	/// </summary>
	/// <param name="account">Аккаунт для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Add(Account account, CancellationToken ct = default);

	/// <summary>
	/// Проверяет существование аккаунта по спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для проверки существования аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом проверки существования аккаунта.</returns>
	Task<bool> Exists(AccountSpecification specification, CancellationToken ct = default);

	/// <summary>
	///   Находит аккаунт по спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для поиска аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом поиска аккаунта.</returns>
	Task<Result<Account>> Find(AccountSpecification specification, CancellationToken ct = default);
}
