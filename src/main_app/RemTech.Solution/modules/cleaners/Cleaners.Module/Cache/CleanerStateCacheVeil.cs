using Cleaners.Module.Domain;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StackExchange.Redis;

namespace Cleaners.Module.Cache;

internal sealed class CleanerStateCacheVeil(string key, IDatabase database) : ICleanerVeil
{
    private string _state = string.Empty;

    public void Accept(
        Guid id,
        int cleanedAmount,
        DateTime lastRun,
        DateTime nextRun,
        int waitDays,
        string state,
        int hours,
        int minutes,
        int seconds,
        int itemsDateDayThreshold
    ) => _state = state;

    public ICleaner Behave() => throw new UnsupportedContentTypeException("Use PrintTo method.");

    public Task<ICleaner> BehaveAsync(CancellationToken ct = default) =>
        throw new UnsupportedContentTypeException("Use PrintTo method.");

    public async Task Save()
    {
        await database.StringSetAsync(key, _state);
    }
}
