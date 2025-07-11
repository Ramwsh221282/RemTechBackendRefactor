using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async.Decorators;

public sealed class ResourceDisposingIncreaseProcessed(IAsyncIncreaseProcessed origin)
    : IAsyncIncreaseProcessed
{
    private readonly List<IDisposable> _disposables = [];

    public async Task<Status<ParserStatisticsIncreasement>> Increase(
        AsyncIncreaseProcess increase,
        CancellationToken ct = default
    )
    {
        Status<ParserStatisticsIncreasement> status = await origin.Increase(increase, ct);
        foreach (IDisposable disposable in _disposables)
            disposable.Dispose();
        return status;
    }

    public ResourceDisposingIncreaseProcessed With(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }
}
