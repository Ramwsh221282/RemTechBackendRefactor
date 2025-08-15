using Cleaners.Module.Domain;
using StackExchange.Redis;

namespace Cleaners.Module.Cache;

internal sealed class CleanerStateCache(ConnectionMultiplexer multiplexer) : ICleanerCache
{
    private const string Key = "cleaner_state";

    public async Task Invalidate(ICleaner cleaner)
    {
        await cleaner
            .ProduceOutput()
            .PrintTo(new CleanerStateCacheVeil(Key, multiplexer.GetDatabase()))
            .Save();
    }
}
