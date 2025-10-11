using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

public sealed record TelemetryActionName
{
    public const int MAX_LENGTH = 200;
    public string Value { get; }

    private TelemetryActionName(string value) => Value = value;

    public static Status<TelemetryActionName> Create(string? value) =>
        value switch
        {
            null => Error.Validation("Название действия телеметрии было пустым"),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Название действия телеметрии было пустым"
            ),
            not null when value.Length > MAX_LENGTH => Error.Validation(
                $"Название действия телеметрии превышает длину: {MAX_LENGTH} символов"
            ),
            _ => new TelemetryActionName(value),
        };
}
