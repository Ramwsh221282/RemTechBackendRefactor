using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public readonly record struct SubscribedParserId
{
    public Guid Value { get; private init; }

    public SubscribedParserId() => Value = Guid.NewGuid();

    private SubscribedParserId(Guid value) => Value = value;

    public static Result<SubscribedParserId> Create(Guid value) =>
        value == Guid.Empty
            ? Error.Validation("Идентификатор подписки парсера не может быть пустым.")
            : new SubscribedParserId(value);

    public static SubscribedParserId New()
    {
        Guid value = Guid.NewGuid();
        return Create(value).Value;
    }
}
