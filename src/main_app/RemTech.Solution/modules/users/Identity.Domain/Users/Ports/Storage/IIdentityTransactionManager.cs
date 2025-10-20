namespace Identity.Domain.Users.Ports.Storage;

public interface IIdentityTransactionManager
{
    Task<IIdentityTransactionScope> BeginTransaction(CancellationToken ct = default);
}
