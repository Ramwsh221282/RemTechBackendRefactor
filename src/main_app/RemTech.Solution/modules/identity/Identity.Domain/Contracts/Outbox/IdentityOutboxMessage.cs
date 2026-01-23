using System.Text.Json;

namespace Identity.Domain.Contracts.Outbox;

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

	public Guid Id { get; } = id;
	public string Type { get; } = type;
	public int RetryCount { get; private set; } = retryCount;
	public DateTime Created { get; } = created;
	public DateTime? Sent { get; private set; } = sent;
	public string Payload { get; } = payload;

	public static IdentityOutboxMessage CreateNew<T>(string type, int retryCount, T payload)
		where T : IOutboxMessagePayload => new(type, retryCount, JsonSerializer.Serialize(payload));

	public static IdentityOutboxMessage CreateNew<T>(string type, T payload)
		where T : IOutboxMessagePayload => new(type, 0, JsonSerializer.Serialize(payload));

	public void MarkSent() => Sent = DateTime.UtcNow;

	public void IncreaseRetryCount() => RetryCount++;

	public IdentityOutboxMessage Clone() => new(this);
}
