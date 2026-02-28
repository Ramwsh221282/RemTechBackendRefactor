using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using StackExchange.Redis;
using Timezones.Core.Contracts;
using Timezones.Core.Models;
using TimeZones.Infrastructure;

namespace TimeZones.Infrastructure.Persistence;

internal sealed class CachedRegionDateTimesRepository : IRegionDateTimesRepository
{
    public CachedRegionDateTimesRepository(IOptions<CachingOptions> options)
    {
        _options = options;                
    }

    private const string TIMEZONES_ARRAY_KEY = "REGION_DATE_TIMES_ARRAY";
    private readonly IOptions<CachingOptions> _options;
    private static readonly Expiration _expiration = new(TimeSpan.FromMinutes(5));
    private ConnectionMultiplexer? _multiplexer;
    private IDatabase? _database;    

    public async Task Refresh(IReadOnlyList<RegionLocalDateTime> dateTimes)
    {
        string json = dateTimes.ToJson();
        IDatabase database = await Database();
        await database.StringSetAsync(TIMEZONES_ARRAY_KEY, json, _expiration);
    }

    public async Task<IReadOnlyList<RegionLocalDateTime>> ProvideDateTimes()
    {
        IDatabase database = await Database();
        string? json = await database.StringGetAsync(TIMEZONES_ARRAY_KEY);
        return string.IsNullOrWhiteSpace(json) ? [] : RegionLocalDateTime.ToList(json);
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
