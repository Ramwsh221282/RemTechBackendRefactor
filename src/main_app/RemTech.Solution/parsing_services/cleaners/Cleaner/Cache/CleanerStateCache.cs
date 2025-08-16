using StackExchange.Redis;

namespace Cleaner.Cache;

internal sealed class CleanerStateCache(ConnectionMultiplexer multiplexer)
{
    private const string Key = "cleaner_state";

    public async Task<bool> IsStateDisabled()
    {
        IDatabase db = multiplexer.GetDatabase();
        string? data = await db.StringGetAsync(Key);
        if (string.IsNullOrEmpty(data))
            return false;
        return data == "Отключен";
    }
}
