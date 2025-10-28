namespace RemTech.Core.Shared.Transactions;

public interface ITransactionManager
{
    Task<ITransaction> Create(CancellationToken ct = default);
}
