using System.Diagnostics;
using System.Text.Json;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Cleaner.WebApi.Storages;

public sealed class WorkTimeStorage(RedisCache cache)
{
    private const string Key = "Cleaner_worktime";
    private readonly IDatabase _database = cache.Database;

    public async Task Invalidate(Stopwatch sw)
    {
        long totalSeconds = (long)sw.Elapsed.TotalSeconds;
        IEnumerable<long> current = await GetSeconds();
        current = [totalSeconds, .. current];
        string json = MakeJson(current);
        await _database.StringSetAsync(Key, json);
    }

    public async Task<long> GetTotalElapsed()
    {
        IEnumerable<long> current = await GetSeconds();
        return current.Sum();
    }

    public async Task Reset() => await _database.KeyDeleteAsync(Key);

    private async Task<IEnumerable<long>> GetSeconds()
    {
        string? json = await _database.StringGetAsync(Key);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        IEnumerable<long> seconds = JsonSerializer.Deserialize<IEnumerable<long>>(json)!;
        return seconds;
    }

    private string MakeJson(IEnumerable<long> seconds)
    {
        return JsonSerializer.Serialize(seconds);
    }
}
