using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.TelemetryContext.ValueObjects;

/// <summary>
/// Статусы результата действия
/// </summary>
public sealed record TelemetryActionStatus
{
    public static readonly TelemetryActionStatus Failure = new("Ошибка");
    public static readonly TelemetryActionStatus Success = new("Успех");
    private static readonly TelemetryActionStatus[] _allowed = [Failure, Success];

    public const int MaxLength = 100;
    public string Value { get; }

    private TelemetryActionStatus(string value) => Value = value;

    public static Status<TelemetryActionStatus> Create(string? value) =>
        value switch
        {
            null => Error.Validation("Статус действия записи телеметрии был пустым."),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Статус действия записи телеметрии был пустым."
            ),
            not null when StringNotMatchesStatus(value) => Error.Validation(
                $"Статус операции телеметрии: {value} не поддерживается."
            ),
            not null when value.Length > MaxLength => Error.Validation(
                $"Статус действия телеметрии превышает длину {MaxLength} символов."
            ),
            _ => new TelemetryActionStatus(value),
        };

    private static bool StringNotMatchesStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return !_allowed.Any(s => s.Value.Equals(value));
    }
}
