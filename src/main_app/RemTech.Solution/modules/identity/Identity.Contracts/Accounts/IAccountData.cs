namespace Identity.Contracts.Accounts;

public interface IAccountData
{
    Guid Id { get; }
    string Name { get; }
    string Email { get; }
    string Password { get; }
    bool Activated { get; }
}