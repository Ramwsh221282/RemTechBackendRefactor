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
        return id == Guid.Empty ? (Result<SubscribedParserLinkId>)Error.Validation("Идентификатор ссылки на парсер не может быть пустым.") : (Result<SubscribedParserLinkId>)new SubscribedParserLinkId(id);
    }
}
