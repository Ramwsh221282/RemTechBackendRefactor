namespace Identity.Domain.Contracts.Outbox;

/// <summary>
/// Интерфейс для работы с исходящей очередью сообщений модуля аккаунтов.
/// </summary>
public interface IAccountModuleOutbox
{
	/// <summary>
	/// Добавляет сообщение в исходящую очередь.
	/// </summary>
	/// <param name="message">Сообщение для добавления в очередь.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления сообщения.</returns>
	Task Add(IdentityOutboxMessage message, CancellationToken ct = default);

	/// <summary>
	/// Получает сообщения из исходящей очереди по спецификации.
	/// </summary>
	/// <param name="spec">Спецификация для фильтрации сообщений.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Массив сообщений, соответствующих спецификации.</returns>
	Task<IdentityOutboxMessage[]> GetMany(OutboxMessageSpecification spec, CancellationToken ct = default);
}
