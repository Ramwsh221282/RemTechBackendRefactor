namespace RemTech.SharedKernel.Core.InfrastructureContracts;

/// <summary>
/// Интерфейс для публикации сообщений.
/// </summary>
/// <typeparam name="TMessage">Тип сообщения.</typeparam>
public interface IMessagePublisher<in TMessage>
	where TMessage : Message
{
	/// <summary>
	/// Публикует сообщение.
	/// </summary>
	/// <param name="message">Сообщение для публикации.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию публикации.</returns>
	Task Publish(TMessage message, CancellationToken ct = default);
}
