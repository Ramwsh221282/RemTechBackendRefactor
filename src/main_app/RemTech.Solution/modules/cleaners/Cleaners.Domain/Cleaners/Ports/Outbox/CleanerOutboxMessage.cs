using System.Text.Json;

namespace Cleaners.Domain.Cleaners.Ports.Outbox;

public sealed class CleanerOutboxMessage
{
    public required Guid Id { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime? Processed { get; set; }
    public required string Type { get; set; }
    public required string Content { get; set; }

    public required int ProcessedAttempts { get; set; }

    public required string? LastError { get; set; }

    public static CleanerOutboxMessage Create<T>(T @object)
        where T : class
    {
        DateTime created = DateTime.UtcNow;
        Guid id = Guid.NewGuid();
        string type = @object.GetType().Name;
        string content = JsonSerializer.Serialize(@object);
        return new CleanerOutboxMessage()
        {
            Id = id,
            Created = created,
            Processed = null,
            Type = type,
            Content = content,
            ProcessedAttempts = 0,
            LastError = null,
        };
    }
}
