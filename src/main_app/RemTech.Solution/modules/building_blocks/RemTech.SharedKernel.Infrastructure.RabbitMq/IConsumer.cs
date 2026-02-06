using RabbitMQ.Client;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

/// <summary>
/// Интерфейс для потребителя сообщений RabbitMQ.
/// </summary>
public interface IConsumer
{
	/// <summary>
	/// Инициализирует канал RabbitMQ для потребителя.
	/// </summary>
	/// <param name="connection">Подключение к RabbitMQ.</param>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию инициализации канала.</returns>
	Task InitializeChannel(IConnection connection, CancellationToken ct = default);

	/// <summary>
	/// Запускает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию запуска потребления сообщений.</returns>
	Task StartConsuming(CancellationToken ct = default);

	/// <summary>
	/// Останавливает потребление сообщений.
	/// </summary>
	/// <param name="ct">Токен отмены для прерывания операции.</param>
	/// <returns>Задача, представляющая асинхронную операцию остановки потребления сообщений.</returns>
	Task Shutdown(CancellationToken ct = default);
}
