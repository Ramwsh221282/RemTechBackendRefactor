namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public sealed record TelemetryActionDetails
{
    public TelemetryActionName Name { get; }
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
