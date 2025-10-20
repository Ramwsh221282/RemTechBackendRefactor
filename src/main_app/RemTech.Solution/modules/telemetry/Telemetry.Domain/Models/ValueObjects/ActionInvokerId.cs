using RemTech.Core.Shared.Result;

namespace Telemetry.Domain.Models.ValueObjects;

/// <summary>
/// Идентификатор того, кто вызвал действие.
/// </summary>
public readonly record struct ActionInvokerId
{
    public Guid Value { get; }

    public ActionInvokerId()
    {
        Value = Guid.NewGuid();
    }

    private ActionInvokerId(Guid value)
    {
        Value = value;
    }

    public static Status<ActionInvokerId> Create(Guid? value) =>
        value switch
        {
            null => Error.Validation("Идентификатор пользователя пустой для записи телеметрии"),
            not null when value == Guid.Empty => Error.Validation(
                "Идентификатор пользователя пустой для записи телеметрии"
            ),
            _ => Create(value.Value),
        };

    public static Status<ActionInvokerId> Create(Guid value) =>
        value switch
        {
            _ when value == Guid.Empty => Error.Validation(
                "Идентификатор пользователя пустой для записи телеметрии"
            ),
            _ => new ActionInvokerId(value),
        };
}
