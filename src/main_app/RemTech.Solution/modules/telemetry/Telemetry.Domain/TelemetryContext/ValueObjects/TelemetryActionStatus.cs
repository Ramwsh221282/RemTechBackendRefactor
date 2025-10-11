using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public sealed record TelemetryActionStatus
{
    public const int MAX_LENGTH = 100;
    public string Value { get; }

    private TelemetryActionStatus(string value) => Value = value;

    public static Status<TelemetryActionStatus> Create(string? value) =>
        value switch
        {
            null => Error.Validation("Статус действия записи телеметрии был пустым."),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Статус действия записи телеметрии был пустым."
            ),
            not null when value.Length > MAX_LENGTH => Error.Validation(
                $"Статус действия телеметрии превышает длину {MAX_LENGTH} символов."
            ),
            _ => new TelemetryActionStatus(value),
        };
}
