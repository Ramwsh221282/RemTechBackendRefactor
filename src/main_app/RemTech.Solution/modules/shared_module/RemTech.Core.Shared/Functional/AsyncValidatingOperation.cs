using RemTech.Result.Library;

namespace RemTech.Core.Shared.Functional;

public sealed class AsyncValidatingOperation(IMaybeError error)
{
    public Task<Status<T>> Process<T>(Task<Status<T>> processFunction) =>
        error.Errored() ? Task.FromResult(Status<T>.Failure(error.Error())) : processFunction;
}
