namespace Identity.Domain.Contracts.Outbox;

/// <summary>
/// Реестр публикации сообщений исходящей очереди.
/// </summary>
/// <param name="publishers">Коллекция издателей сообщений исходящей очереди.</param>
public sealed class OutboxMessagesRegistry(IEnumerable<IAccountOutboxMessagePublisher> publishers)
{
	/// <summary>
	/// Публикует коллекцию сообщений исходящей очереди.
	/// </summary>
	/// <param name="messages">Коллекция сообщений исходящей очереди для публикации.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Задача публикации.</returns>
	public async Task Publish(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default)
	{
		foreach (IdentityOutboxMessage message in messages)
		{
			foreach (IAccountOutboxMessagePublisher publisher in publishers)
			{
				if (publisher.CanPublish(message))
					await publisher.Publish(message, ct);
			}
		}
	}
}
