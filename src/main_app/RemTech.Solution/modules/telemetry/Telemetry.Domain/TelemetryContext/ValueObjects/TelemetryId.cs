using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public readonly record struct TelemetryId
{
    public Guid Value { get; }

    public TelemetryId() => Value = Guid.NewGuid();

    private TelemetryId(Guid value) => Value = value;

    public static Status<TelemetryId> Create(Guid? value) =>
        value switch
        {
            null => Error.Validation("Идентификатор записи действия не может быть пустым."),
            not null when value.Value == Guid.Empty => Error.Validation(
                "Идентификатор записи действия не может быть пустым."
            ),
            _ => new TelemetryId(value.Value),
        };
}
