using RemTech.Core.Shared.Result;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Domain.TelemetryContext.Contracts;

/// <summary>
/// Контракт взаимодействия с хранилищем записанных действий.
/// </summary>
public interface ITelemetryRecordsRepository
{
    /// <summary>
    /// Добавить действие в хранилище
    /// </summary>
    Task Add(TelemetryRecord record, CancellationToken ct = default);

    /// <summary>
    /// Получить действие по его ИД
    /// </summary>
    Task<Status<TelemetryRecord>> GetById(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Получить действие по его ИД (типизированный)
    /// </summary>
    Task<Status<TelemetryRecord>> GetById(
        TelemetryRecordId recordId,
        CancellationToken ct = default
    );

    /// <summary>
    /// Получить действия по названию
    /// </summary>
    Task<IEnumerable<TelemetryRecord>> GetByName(string name, CancellationToken ct = default);

    /// <summary>
    /// Получить действия по названию (типизированный)
    /// </summary>
    Task<IEnumerable<TelemetryRecord>> GetByName(
        TelemetryActionName name,
        CancellationToken ct = default
    );
}
