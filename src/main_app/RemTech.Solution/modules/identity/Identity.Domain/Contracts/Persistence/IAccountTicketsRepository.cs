using Identity.Domain.Tickets;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

/// <summary>
/// Интерфейс репозитория для управления тикетами аккаунтов.
/// </summary>
public interface IAccountTicketsRepository
{
	/// <summary>
	/// Добавляет новый тикет аккаунта в репозиторий.
	/// </summary>
	/// <param name="ticket">Тикет аккаунта для добавления.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача.</returns>
	Task Add(AccountTicket ticket, CancellationToken ct = default);

	/// <summary>
	/// Находит тикет аккаунта по спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для поиска тикета аккаунта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача с результатом поиска тикета аккаунта.</returns>
	Task<Result<AccountTicket>> Find(AccountTicketSpecification specification, CancellationToken ct = default);
}
