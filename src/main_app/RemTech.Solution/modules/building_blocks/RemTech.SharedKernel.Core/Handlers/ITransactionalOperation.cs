using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.Handlers;

public interface ITransactionalOperation
{
    Task<Result<U>> Execute<U>(AsyncOperationResult<U> operation, CancellationToken ct = default);
}