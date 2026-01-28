using System.Text.Json;

namespace Identity.Domain.Contracts.Outbox;

/// <summary>
/// Сообщение исходящей очереди модуля идентификации.
/// </summary>
/// <param name="id">Идентификатор сообщения.</param>
/// <param name="type">Тип сообщения.</param>
/// <param name="retryCount">Количество попыток отправки.</param>
/// <param name="created">Дата и время создания сообщения.</param>
/// <param name="sent">Дата и время отправки сообщения.</param>
/// <param name="payload">Полезная нагрузка сообщения.</param>
public sealed class IdentityOutboxMessage(
	Guid id,
	string type,
	int retryCount,
	DateTime created,
	DateTime? sent,
	string payload
)
{
	private IdentityOutboxMessage(IdentityOutboxMessage message)
		: this(message.Id, message.Type, message.RetryCount, message.Created, message.Sent, message.Payload) { }

	private IdentityOutboxMessage(string type, int retryCount, string payload)
		: this(Guid.NewGuid(), type, retryCount, DateTime.UtcNow, null, payload) { }

	/// <summary>
	/// Идентификатор сообщения.
	/// </summary>
	public Guid Id { get; } = id;

	/// <summary>
	/// Тип сообщения.
	/// </summary>
	public string Type { get; } = type;

	/// <summary>
	/// Количество попыток отправки.
	/// </summary>
	public int RetryCount { get; private set; } = retryCount;

	/// <summary>
	/// Дата и время создания сообщения.
	/// </summary>
	public DateTime Created { get; } = created;

	/// <summary>
	/// Дата и время отправки сообщения.
	/// </summary>
	public DateTime? Sent { get; private set; } = sent;

	/// <summary>
	/// Полезная нагрузка сообщения.
	/// </summary>
	public string Payload { get; } = payload;

	/// <summary>
	///  Создает новое сообщение исходящей очереди.
	/// </summary>
	/// <typeparam name="T">Тип с полезной нагрузкой сообщения.</typeparam>
	/// <param name="type">Тип сообщения.</param>
	/// <param name="retryCount">Количество попыток отправки.</param>
	/// <param name="payload">Полезная нагрузка сообщения.</param>
	/// <returns>Новое сообщение исходящей очереди.</returns>
	public static IdentityOutboxMessage CreateNew<T>(string type, int retryCount, T payload)
		where T : IOutboxMessagePayload => new(type, retryCount, JsonSerializer.Serialize(payload));

	/// <summary>
	/// Создает новое сообщение исходящей очереди с нулевым количеством попыток отправки.
	/// </summary>
	/// <typeparam name="T">Тип с полезной нагрузкой сообщения.</typeparam>
	/// <param name="type">Тип сообщения.</param>
	/// <param name="payload">Полезная нагрузка сообщения.</param>
	/// <returns>Новое сообщение исходящей очереди.</returns>
	public static IdentityOutboxMessage CreateNew<T>(string type, T payload)
		where T : IOutboxMessagePayload => new(type, 0, JsonSerializer.Serialize(payload));

	/// <summary>
	/// Отмечает сообщение как отправленное, устанавливая дату и время отправки.
	/// </summary>
	public void MarkSent() => Sent = DateTime.UtcNow;

	/// <summary>
	/// Увеличивает количество попыток отправки на единицу.
	/// </summary>
	public void IncreaseRetryCount() => RetryCount++;

	/// <summary>
	/// Клонирует сообщение исходящей очереди.
	/// </summary>
	/// <returns>Клон сообщения исходящей очереди.</returns>
	public IdentityOutboxMessage Clone() => new(this);
}
