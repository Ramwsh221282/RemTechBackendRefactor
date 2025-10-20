using Identity.Domain.Users.ValueObjects;

namespace Identity.Domain.Users;

public sealed class User
{
    public required UserId Id { get; init; }
    public required UserLogin Login { get; init; }
    public required UserEmail Email { get; init; }
    public required HashedUserPassword Password { get; init; }
    public required bool EmailConfirmed { get; init; }
}
