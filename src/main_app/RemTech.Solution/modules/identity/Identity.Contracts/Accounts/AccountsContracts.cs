using Identity.Contracts.Shared;

namespace Identity.Contracts.Accounts;

public interface IAccountDecrypter : IDecrypter<IAccount>;
public interface IAccountEncrypter : IEncrypter<IAccount>;
public sealed record AccountQueryArgs(
    Guid? Id = null, 
    string? Name = null, 
    string? Email = null,
    bool WithLock = false) : PersisterQueryArgs;
public interface IAccountPersister : IPersister<IAccount, AccountQueryArgs>;