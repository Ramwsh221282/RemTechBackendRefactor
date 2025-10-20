using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.Models.ValueObjects;

/// <summary>
/// Название действия
/// </summary>
public sealed record ActionName
{
    public const int MaxLength = 200;
    public string Value { get; }

    private ActionName(string value) => Value = value;

    public static ActionName Unknown() => new("Неизвестно");

    public static Status<ActionName> Create(string? value) =>
        value switch
        {
            null => Error.Validation("Название действия телеметрии было пустым"),
            not null when string.IsNullOrWhiteSpace(value) => Error.Validation(
                "Название действия телеметрии было пустым"
            ),
            not null when value.Length > MaxLength => Error.Validation(
                $"Название действия телеметрии превышает длину: {MaxLength} символов"
            ),
            _ => new ActionName(value),
        };
}
