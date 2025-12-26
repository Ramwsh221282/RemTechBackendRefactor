using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinks.Models;

public readonly record struct SubscribedParserLinkId
{
    public Guid Value { get; private init; }
    public SubscribedParserLinkId() => Value = Guid.NewGuid();
    private SubscribedParserLinkId(Guid id) => Value = id;
    public static SubscribedParserLinkId New() => new();

    public static Result<SubscribedParserLinkId> From(Guid id)
    {
        if (id == Guid.Empty) 
            return Error.Validation("Идентификатор ссылки на парсер не может быть пустым.");
        return new SubscribedParserLinkId(id);
    }
}