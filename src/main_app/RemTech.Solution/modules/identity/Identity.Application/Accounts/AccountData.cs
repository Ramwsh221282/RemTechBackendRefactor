using Identity.Contracts.Accounts;

namespace Identity.Application.Accounts;

public sealed record AccountData(
    Guid Id,
    string Name,
    string Email,
    string Password,
    bool Activated) : IAccountData
{
    public static AccountData New(string name, string email, string password)
    {
        return new AccountData(Guid.NewGuid(), name, email, password, false);
    }

    public static AccountData Copy(
        IAccountData data,
        Guid? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        bool? activated = null
        )
    {
        Guid cid = id ?? data.Id;
        string cName = name ?? data.Name;
        string cEmail = email ?? data.Email;
        string cPass = password ?? data.Password;
        bool cAct = activated ?? data.Activated;
        return new AccountData(cid, cName, cEmail, cPass, cAct);
    }
    
    public static AccountData Copy(
        IAccountRepresentation representation,
        Guid? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        bool? activated = null
    )
    {
        return Copy(representation.Data, id, name, email, password, activated);
    }
    
    public static AccountData Copy(
        IAccount account,
        Guid? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        bool? activated = null
    )
    {
        return Copy(account.Represent(AccountRepresentation.Empty()), id, name, email, password, activated);
    }
}