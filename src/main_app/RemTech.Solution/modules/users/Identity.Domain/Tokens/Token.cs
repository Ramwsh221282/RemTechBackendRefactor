using RemTech.Core.Shared.Result;

namespace Identity.Domain.Tokens;

public readonly record struct Token
{
    public Guid Id { get; }

    public Token() => Id = Guid.NewGuid();

    private Token(Guid id) => Id = id;

    public static Status<Token> Create(Guid id)
    {
        if (id == Guid.Empty)
            return Error.Validation("Токен невалиден.");
        return new Token(id);
    }
}
