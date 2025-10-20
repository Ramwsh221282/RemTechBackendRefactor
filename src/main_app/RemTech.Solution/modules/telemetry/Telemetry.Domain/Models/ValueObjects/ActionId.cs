using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.Models.ValueObjects;

/// <summary>
/// Идентификатор того, кто вызвал действие (ид пользователя)
/// </summary>
public readonly record struct ActionId
{
    public Guid Value { get; }

    public ActionId() => Value = Guid.NewGuid();

    private ActionId(Guid value) => Value = value;

    public static Status<ActionId> Create(Guid? value) =>
        value switch
        {
            null => Error.Validation("Идентификатор записи действия не может быть пустым."),
            not null when value.Value == Guid.Empty => Error.Validation(
                "Идентификатор записи действия не может быть пустым."
            ),
            _ => new ActionId(value.Value),
        };
}
