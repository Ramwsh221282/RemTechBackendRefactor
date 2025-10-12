using RemTech.Result.Pattern;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

/// <summary>
/// Дата записи действия телеметрии
/// </summary>
public readonly record struct TelemetryRecordDate
{
    public DateTime OccuredAt { get; }

    public TelemetryRecordDate() => OccuredAt = DateTime.UtcNow;

    private TelemetryRecordDate(DateTime occuredAt) => OccuredAt = occuredAt;

    public static Result<TelemetryRecordDate> Create(DateTime? occuredAt) =>
        occuredAt == null ? new TelemetryRecordDate() : Create(occuredAt.Value);

    public static Result<TelemetryRecordDate> Create(DateTime occuredAt) =>
        occuredAt switch
        {
            _ when occuredAt == DateTime.MaxValue => Error.Validation(
                "Дата записи телеметрии некорректна."
            ),
            _ when occuredAt == DateTime.MinValue => Error.Validation(
                "Дата записи телеметрии некорректна."
            ),
            _ => new TelemetryRecordDate(occuredAt),
        };
}
