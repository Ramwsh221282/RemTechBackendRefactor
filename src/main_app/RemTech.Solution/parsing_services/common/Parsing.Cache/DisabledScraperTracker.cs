using StackExchange.Redis;

namespace Parsing.Cache;

internal sealed class DisabledScraperTracker(ConnectionMultiplexer multiplexer)
    : IDisabledScraperTracker
{
    public async Task<bool> HasBeenDisabled(string name, string type)
    {
        IDatabase database = multiplexer.GetDatabase();
        string? record = await database.StringGetAsync(MakeKey(name, type));
        Console.WriteLine(record);
        if (string.IsNullOrWhiteSpace(record))
            return false;
        if (record == "Отключен")
            return true;
        return false;
    }

    private static string MakeKey(string name, string type)
    {
        string key = $"state_{name}_{type}";
        return key;
    }
}
