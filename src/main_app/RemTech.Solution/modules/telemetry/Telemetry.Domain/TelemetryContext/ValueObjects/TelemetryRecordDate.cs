using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public readonly record struct TelemetryRecordDate
{
    public DateTime OccuredAt { get; }

    public TelemetryRecordDate() => OccuredAt = DateTime.UtcNow;

    private TelemetryRecordDate(DateTime occuredAt) => OccuredAt = occuredAt;

    public static Status<TelemetryRecordDate> Create(DateTime? occuredAt) =>
        occuredAt switch
        {
            null => Error.Validation("Дата записи телеметрии была пустой."),
            not null when occuredAt.Value == DateTime.MaxValue => Error.Validation(
                "Дата записи телеметрии некорректна."
            ),
            not null when occuredAt.Value == DateTime.MinValue => Error.Validation(
                "Дата записи телеметрии некорректна."
            ),
            _ => new TelemetryRecordDate(occuredAt.Value),
        };
}
