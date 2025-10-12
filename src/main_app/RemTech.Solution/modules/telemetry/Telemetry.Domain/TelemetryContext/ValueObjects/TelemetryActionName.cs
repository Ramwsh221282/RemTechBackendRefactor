using RemTech.Result.Pattern;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

/// <summary>
/// Название действия
/// </summary>
public sealed record TelemetryActionName
{
    public const int MaxLength = 200;
    public string Value { get; }

    private TelemetryActionName(string value) => Value = value;

    public static TelemetryActionName Unknown() => new("Неизвестно");

    public static Result<TelemetryActionName> Create(string? value) =>
        value switch
        {
            null => Error.Validation("Название действия телеметрии было пустым"),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Название действия телеметрии было пустым"
            ),
            not null when value.Length > MaxLength => Error.Validation(
                $"Название действия телеметрии превышает длину: {MaxLength} символов"
            ),
            _ => new TelemetryActionName(value),
        };
}
