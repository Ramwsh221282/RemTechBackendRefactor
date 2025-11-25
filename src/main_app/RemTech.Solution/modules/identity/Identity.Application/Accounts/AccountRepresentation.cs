using Identity.Contracts.Accounts;

namespace Identity.Application.Accounts;

public sealed record AccountRepresentation(Guid Id, string Name, string Email, string Password, bool Activated)
    : IAccountRepresentation
{
    public static AccountRepresentation Empty() => new(
        Guid.Empty, 
        string.Empty, 
        string.Empty, 
        string.Empty, 
        false);

    public IAccountData Data => new AccountData(Id, Name, Email, Password, Activated);

    public IAccountRepresentation AddId(Guid id) =>
        this with { Id = id };

    public IAccountRepresentation AddName(string name) =>
        this with { Name = name };

    public IAccountRepresentation AddEmail(string email) =>
        this with { Email = email };

    public IAccountRepresentation AddPassword(string password) =>
        this  with { Password = password };

    public IAccountRepresentation AddActivated(bool activated) =>
        this with { Activated = activated };
}