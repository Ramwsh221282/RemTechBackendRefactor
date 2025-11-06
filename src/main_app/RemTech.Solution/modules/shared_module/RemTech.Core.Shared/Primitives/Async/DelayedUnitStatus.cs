using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives.Async;

public sealed class DelayedUnitStatus
{
    private readonly TaskCompletionSource<Status<Unit>> _tcs = new();
    private bool _hasSomethingFailed;

    public void Accept(Status<Unit> status)
    {
        if (_hasSomethingFailed)
            return;

        if (status.IsFailure)
            _hasSomethingFailed = true;

        _tcs.SetResult(status);
    }

    public async Task<Status<Unit>> Read() => await _tcs.Task;
}