using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public interface ITransactionalOperation
{
    Task<Result<U>> Execute<U>(AsyncOperation<U> operation, CancellationToken ct = default);
}