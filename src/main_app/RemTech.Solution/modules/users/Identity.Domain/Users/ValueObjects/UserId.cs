using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.ValueObjects;

public readonly record struct UserId
{
    public Guid Id { get; }

    public UserId() => Id = Guid.NewGuid();

    private UserId(Guid id) => Id = id;

    public static Status<UserId> Create(Guid id) =>
        id == Guid.Empty
            ? Error.Validation("Идентификатор пользователя был пустым")
            : new UserId(id);
}
