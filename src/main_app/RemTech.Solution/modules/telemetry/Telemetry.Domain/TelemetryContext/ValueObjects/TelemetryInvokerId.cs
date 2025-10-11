using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public readonly record struct TelemetryInvokerId
{
    public Guid Value { get; }

    public TelemetryInvokerId()
    {
        Value = Guid.NewGuid();
    }

    private TelemetryInvokerId(Guid value)
    {
        Value = value;
    }

    public static Status<TelemetryInvokerId> Create(Guid? value) =>
        value switch
        {
            null => Error.Validation("Идентификатор пользователя пустой для записи телеметрии"),
            not null when value == Guid.Empty => Error.Validation(
                "Идентификатор пользователя пустой для записи телеметрии"
            ),
            _ => Create(value.Value),
        };

    public static Status<TelemetryInvokerId> Create(Guid value) =>
        value switch
        {
            _ when value == Guid.Empty => Error.Validation(
                "Идентификатор пользователя пустой для записи телеметрии"
            ),
            _ => new TelemetryInvokerId(value),
        };
}
