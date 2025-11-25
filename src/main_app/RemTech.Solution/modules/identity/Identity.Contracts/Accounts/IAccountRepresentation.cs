namespace Identity.Contracts.Accounts;

public interface IAccountRepresentation
{
    public IAccountData Data { get; }
    IAccountRepresentation AddId(Guid id);
    IAccountRepresentation AddName(string name);
    IAccountRepresentation AddEmail(string email);
    IAccountRepresentation AddPassword(string password);
    IAccountRepresentation AddActivated(bool activated);
}