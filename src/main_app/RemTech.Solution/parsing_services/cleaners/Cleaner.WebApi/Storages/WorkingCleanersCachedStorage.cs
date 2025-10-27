using System.Text.Json;
using Cleaner.WebApi.Models;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Cleaner.WebApi.Storages;

public sealed class WorkingCleanersCachedStorage(RedisCache cache) : IWorkingCleanersStorage
{
    private readonly IDatabase _db = cache.Database;
    private const string Key = "WORKING_CLEANERS";

    public async Task<WorkingCleaner?> Get()
    {
        string? json = await _db.StringGetAsync(Key);
        return json == null ? null : Cleaner(json);
    }

    public async Task Invalidate(WorkingCleaner cleaner)
    {
        string json = CleanerJson(cleaner);
        await _db.StringSetAsync(Key, json);
    }

    public async Task Remove() => await _db.KeyDeleteAsync(Key);

    private string CleanerJson(WorkingCleaner cleaner)
    {
        string json = JsonSerializer.Serialize(cleaner);
        return json;
    }

    private WorkingCleaner Cleaner(string json)
    {
        return JsonSerializer.Deserialize<WorkingCleaner>(json)!;
    }
}
