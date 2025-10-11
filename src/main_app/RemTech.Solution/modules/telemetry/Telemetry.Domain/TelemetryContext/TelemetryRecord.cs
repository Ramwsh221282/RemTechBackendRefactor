using Telemetry.Domain.TelemetryContext.Contracts;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Domain.TelemetryContext;

public sealed class TelemetryRecord
{
    public TelemetryRecordId RecordId { get; }
    public TelemetryInvokerId InvokerId { get; }
    public TelemetryActionDetails Details { get; }
    public TelemetryActionStatus Status { get; }
    public TelemetryRecordDate OccuredAt { get; }

    public TelemetryRecord(
        TelemetryInvokerId invokerId,
        TelemetryActionDetails details,
        TelemetryActionStatus status,
        TelemetryRecordDate occuredAt,
        TelemetryRecordId? recordId = null
    )
    {
        RecordId = recordId ?? new TelemetryRecordId();
        InvokerId = invokerId;
        Details = details;
        Status = status;
        OccuredAt = occuredAt;
    }

    public TelemetryRecord(
        TelemetryInvokerId invokerId,
        TelemetryActionName action,
        IEnumerable<TelemetryActionComment> comments,
        TelemetryActionStatus status,
        TelemetryRecordDate occuredAt,
        TelemetryRecordId? id = null
    )
        : this(invokerId, new TelemetryActionDetails(action, comments), status, occuredAt, id) { }

    public TelemetryRecord(
        TelemetryRecordId recordId,
        TelemetryInvokerId invokerId,
        TelemetryActionName action,
        IEnumerable<TelemetryActionComment> comments,
        TelemetryActionStatus status,
        TelemetryRecordDate occuredAt
    )
        : this(invokerId, new TelemetryActionDetails(action, comments), status, occuredAt, recordId)
    { }

    public TelemetryRecord(
        TelemetryRecordId recordId,
        TelemetryInvokerId invokerId,
        TelemetryActionDetails details,
        TelemetryActionStatus status,
        TelemetryRecordDate occuredAt
    )
        : this(invokerId, details, status, occuredAt, recordId) { }

    public async Task Save(
        ITelemetryRecordsRepository repository,
        CancellationToken ct = default
    ) => await repository.Add(this, ct);
}
