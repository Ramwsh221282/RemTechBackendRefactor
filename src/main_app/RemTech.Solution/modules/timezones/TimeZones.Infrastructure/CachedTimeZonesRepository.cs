using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using StackExchange.Redis;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

internal sealed class CachedTimeZonesRepository : ITimeZonesRepository
{
    public CachedTimeZonesRepository(IOptions<CachingOptions> options)
    {
        _options = options;                
    }

    private const string TIMEZONES_ARRAY_KEY = "TIME_ZONES_ARRAY";
    private readonly IOptions<CachingOptions> _options;
    private static readonly Expiration _expiration = new(TimeSpan.FromMinutes(5));
    private ConnectionMultiplexer? _multiplexer;
    private IDatabase? _database;    

    public async Task Refresh(IReadOnlyList<TimeZoneRecord> records)
    {
        string json = TimeZoneRecord.ToJson(records);
        IDatabase database = await Database();
        await database.StringSetAsync(TIMEZONES_ARRAY_KEY, json, _expiration);
    }

    public async Task<Dictionary<string, TimeZoneRecord>> ProvideTimeZones()
    {
        IDatabase database = await Database();
        string? json = await database.StringGetAsync(TIMEZONES_ARRAY_KEY);
        if (json is null)
        {
            return [];
        }

        return TimeZoneRecord.ToDictionary(json);
    }
    
    private async Task<ConnectionMultiplexer> Multiplexer()
    {
        string connectionString = _options.Value.RedisConnectionString;
        return _multiplexer ??= await ConnectionMultiplexer.ConnectAsync(connectionString);
    }

    private async Task<IDatabase> Database()
    {
        ConnectionMultiplexer multiplexer = await Multiplexer();
        _database ??= multiplexer.GetDatabase();
        return _database;
    } 
}
