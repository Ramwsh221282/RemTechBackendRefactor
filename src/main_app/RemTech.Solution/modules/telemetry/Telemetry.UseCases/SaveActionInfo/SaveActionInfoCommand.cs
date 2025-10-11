using RemTech.UseCases.Shared.Cqrs;
using Telemetry.Domain.TelemetryContext;

namespace Telemetry.UseCases.SaveActionInfo;

/// <summary>
/// Команда для выполнения добавления действия.
/// </summary>
public sealed record SaveActionInfoIbCommand : IBCommand<TelemetryRecord>
{
    public IEnumerable<string> Comments { get; }
    public string Name { get; }
    public string Status { get; }
    public Guid InvokerId { get; }
    public DateTime OccuredAt { get; }

    public SaveActionInfoIbCommand(
        IEnumerable<string> comments,
        string name,
        string status,
        Guid invokerId,
        DateTime? occuredAt = null
    )
    {
        Comments = comments;
        Name = name;
        Status = status;
        InvokerId = invokerId;
        OccuredAt = occuredAt ?? DateTime.UtcNow;
    }
}
