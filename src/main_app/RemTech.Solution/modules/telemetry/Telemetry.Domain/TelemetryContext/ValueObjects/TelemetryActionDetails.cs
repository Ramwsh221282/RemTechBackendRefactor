namespace Telemetry.Domain.TelemetryContext.ValueObjects;

/// <summary>
/// Детали действия
/// </summary>
public sealed record TelemetryActionDetails
{
    /// <summary>
    /// Название действия
    /// </summary>
    public TelemetryActionName Name { get; }

    /// <summary>
    /// Список комментариев к действию
    /// </summary>
    public IReadOnlyList<TelemetryActionComment> Comments { get; }

    public TelemetryActionDetails(
        TelemetryActionName name,
        IEnumerable<TelemetryActionComment> comments
    )
    {
        Name = name;
        Comments = [.. comments];
    }
}
