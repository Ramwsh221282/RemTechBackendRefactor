using RemTech.Core.Shared.Result;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Domain.TelemetryContext.Contracts;

public interface ITelemetryRecordsRepository
{
    Task Add(TelemetryRecord record, CancellationToken ct = default);
    Task<Status<TelemetryRecord>> GetById(Guid id, CancellationToken ct = default);
    Task<Status<TelemetryRecord>> GetById(
        TelemetryRecordId recordId,
        CancellationToken ct = default
    );

    Task<IEnumerable<TelemetryRecord>> GetByName(string name, CancellationToken ct = default);

    Task<IEnumerable<TelemetryRecord>> GetByName(
        TelemetryActionName name,
        CancellationToken ct = default
    );
}
