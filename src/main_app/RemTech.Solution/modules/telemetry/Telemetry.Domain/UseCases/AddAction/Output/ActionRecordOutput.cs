namespace Telemetry.Domain.UseCases.AddAction.Output;

public sealed record ActionRecordOutput(
    Guid Id,
    Guid InvokerId,
    string Status,
    string Name,
    DateTime OccuredAt,
    IEnumerable<string> Comments
);
