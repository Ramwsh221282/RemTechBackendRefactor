using RemTech.Result.Library;

namespace RemTech.Core.Shared.Functional;

public sealed class ValidatingOperation(IMaybeError maybeError)
{
    public Status<T> Process<T>(Func<Status<T>> processFunction) =>
        maybeError.Errored() ? Status<T>.Failure(maybeError.Error()) : processFunction();

    public Status Process(Func<Status> processFunction) =>
        maybeError.Errored() ? Status.Failure(maybeError.Error()) : processFunction();
}
