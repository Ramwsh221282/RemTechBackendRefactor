using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.InfrastructureContracts;

public interface ITransactionScope : IDisposable, IAsyncDisposable
{
    public Task<Result> Commit(CancellationToken ct = default);
}
