using Telemetry.Domain.TelemetryContext.Contracts;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Domain.TelemetryContext;

/// <summary>
/// Запись действия пользователя
/// </summary>
public sealed class TelemetryRecord
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public TelemetryRecordId RecordId { get; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public TelemetryInvokerId InvokerId { get; }

    /// <summary>
    /// Детальная информация о записи пользователя
    /// </summary>
    public TelemetryActionDetails Details { get; }

    /// <summary>
    /// Статус операции (успешный или ошибка)
    /// </summary>
    public TelemetryActionStatus Status { get; }

    /// <summary>
    /// Дата создания записи
    /// </summary>
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
