namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface ITransactionSource
{
	public Task<ITransactionScope> BeginTransaction(CancellationToken ct = default);
}
