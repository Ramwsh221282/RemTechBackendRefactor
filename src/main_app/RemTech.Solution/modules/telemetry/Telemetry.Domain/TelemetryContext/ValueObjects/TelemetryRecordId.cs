using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public readonly record struct TelemetryRecordId
{
    public Guid Value { get; }

    public TelemetryRecordId() => Value = Guid.NewGuid();

    private TelemetryRecordId(Guid value) => Value = value;

    public static Status<TelemetryRecordId> Create(Guid? value) =>
        value switch
        {
            null => Error.Validation("Идентификатор записи действия не может быть пустым."),
            not null when value.Value == Guid.Empty => Error.Validation(
                "Идентификатор записи действия не может быть пустым."
            ),
            _ => new TelemetryRecordId(value.Value),
        };
}
