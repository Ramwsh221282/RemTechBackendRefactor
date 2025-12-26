namespace Telemetry.Adapter.Storage.DataModels;

public sealed class ActionRecordDataModel
{
    public required Guid Id { get; init; }
    public required Guid InvokerId { get; init; }
    public required string Status { get; init; }
    public required string Name { get; init; }
    public required DateTime OccuredAt { get; init; }
    public required IEnumerable<string> Comments { get; init; }
}
