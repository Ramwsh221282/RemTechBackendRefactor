using RemTech.ParsersManagement.Core.Common.Errors;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Decorators;

public sealed class AsyncValidatingOperation(IMaybeError error)
{
    public Task<Status<T>> Process<T>(Task<Status<T>> processFunction) =>
        error.Errored() ? Task.FromResult(Status<T>.Failure(error.Error())) : processFunction;
}
