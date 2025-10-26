using System.Text.Json;
using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Ports;
using Cleaners.Domain.Cleaners.Ports.Cache;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Cleaners.Adapters.Cache;

public sealed class CleanersCachedStorage(RedisCache cache) : ICleanersCachedStorage
{
    private const string Key = "CLEANER";
    private readonly IDatabase _database = cache.Database;

    public async Task<Status<Cleaner>> Get(Guid id)
    {
        string? serialized = await _database.StringGetAsync(Key);
        if (serialized == null)
            return Error.NotFound("Чистильщик не найден в кеше.");

        CleanerCachedModel? dm = JsonSerializer.Deserialize<CleanerCachedModel>(serialized);
        if (dm == null)
            return Error.NotFound("Чистильщик не найден в кеше.");

        var cleaner = dm.ConvertToDomainModel();
        await UpdateIfIdDIffers(id, cleaner);
        return cleaner;
    }

    public async Task Invalidate(Cleaner cleaner)
    {
        CleanerCachedModel dm = cleaner.ConvertToDataModel();
        string serialized = JsonSerializer.Serialize(dm);
        await _database.StringSetAsync(Key, serialized);
    }

    private async Task UpdateIfIdDIffers(Guid id, Cleaner cleaner)
    {
        if (cleaner.Id != id)
            await Invalidate(cleaner);
    }
}
