using RemTech.Result.Library;

namespace RemTech.Core.Shared.Functional;

public sealed class ErrorBag : IMaybeError
{
    private readonly MaybeBag<Error> _bag;

    public ErrorBag() => _bag = new MaybeBag<Error>();

    public ErrorBag(Error error) => _bag = new MaybeBag<Error>(error);

    public ErrorBag(Status status) =>
        _bag = status.IsFailure ? new MaybeBag<Error>(status.Error) : new MaybeBag<Error>();

    public static ErrorBag New<T>(Status<T> status)
    {
        Status upcasted = status;
        return new ErrorBag(upcasted);
    }

    public static ErrorBag New(params Status[] statuses)
    {
        foreach (Status status in statuses)
        {
            if (status.IsFailure)
                return new ErrorBag(status);
        }

        return new ErrorBag();
    }

    public bool Errored() => _bag.Any();

    public Error Error() => _bag.Take();
}
