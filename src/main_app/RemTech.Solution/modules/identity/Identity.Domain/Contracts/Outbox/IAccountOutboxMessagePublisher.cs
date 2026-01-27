namespace Identity.Domain.Contracts.Outbox;

/// <summary>
/// Интерфейс для публикации сообщений из исходящей очереди аккаунтов.
/// </summary>
public interface IAccountOutboxMessagePublisher
{
	/// <summary>
	/// Проверяет, можно ли опубликовать указанное сообщение.
	/// </summary>
	/// <param name="message">Сообщение для проверки возможности публикации.</param>
	/// <returns>True, если сообщение можно опубликовать; в противном случае false.</returns>
	bool CanPublish(IdentityOutboxMessage message);

	/// <summary>
	/// Публикует указанное сообщение.
	/// </summary>
	/// <param name="message">Сообщение для публикации.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации сообщения.</returns>
	Task Publish(IdentityOutboxMessage message, CancellationToken ct = default);
}
