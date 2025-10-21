using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.ValueObjects;

public readonly record struct IdentityTokenId
{
    public Guid Id { get; }

    public IdentityTokenId() => Id = Guid.NewGuid();

    private IdentityTokenId(Guid id) => Id = id;

    public static Status<IdentityTokenId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.Validation("Идентификатор токена некорректный.")
            : new IdentityTokenId(id);
}
