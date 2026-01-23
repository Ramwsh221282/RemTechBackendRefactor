namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface ITransactionSource
{
	Task<ITransactionScope> BeginTransaction(CancellationToken ct = default);
}
