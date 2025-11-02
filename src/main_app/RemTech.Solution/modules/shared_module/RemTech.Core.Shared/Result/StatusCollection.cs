namespace RemTech.Core.Shared.Result;

public sealed class StatusCollection<T>
{
    private readonly IEnumerable<Status<T>> _statuses;

    public StatusCollection(IEnumerable<Status<T>> statuses)
    {
        _statuses = statuses;
    }

    public bool AllValid(out Error error, out IEnumerable<T> values)
    {
        if (_statuses.All(s => s.IsSuccess))
        {
            var firstFailure = _statuses.FirstOrDefault(s => s.IsFailure)!;
            error = firstFailure.Error;
            values = [];
            return false;
        }

        error = Error.None();
        values = _statuses.Select(v => v.Value);
        return true;
    }
}