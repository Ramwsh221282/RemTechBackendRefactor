using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.Models.ValueObjects;

/// <summary>
/// Статусы результата действия
/// </summary>
public sealed record ActionStatus
{
    public static readonly ActionStatus Failure = new("Ошибка");
    public static readonly ActionStatus Success = new("Успех");
    private static readonly ActionStatus[] _allowed = [Failure, Success];

    public const int MaxLength = 100;
    public string Value { get; }

    private ActionStatus(string value) => Value = value;

    public static Status<ActionStatus> Create(string? value) =>
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
            _ => new ActionStatus(value),
        };

    private static bool StringNotMatchesStatus(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return !_allowed.Any(s => s.Value.Equals(value));
    }
}
