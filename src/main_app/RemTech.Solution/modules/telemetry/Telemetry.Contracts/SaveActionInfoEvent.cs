namespace Telemetry.Contracts;

public sealed record SaveActionInfoEvent(
    IEnumerable<string> Comments,
    string Name,
    string Status,
    Guid InvokerId,
    DateTime? OccuredAt
);
