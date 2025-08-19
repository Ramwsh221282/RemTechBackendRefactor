using StackExchange.Redis;

namespace Scrapers.Module.Domain.JournalsContext.Cache;

internal sealed class ActiveScraperJournalCacheOutput
    : IScraperJournalOutputSource<ActiveScraperJournalCacheOutput>
{
    private readonly IDatabase _database;
    private readonly Guid _id;
    private readonly string _parserName;
    private readonly string _parserType;

    public void Behave()
    {
        string key = ActiveScraperJournalsCache.MakeKey(_parserName, _parserType);
        string value = _id.ToString();
        _database.StringSet(key, value);
    }

    public async Task BehaveAsync()
    {
        string key = ActiveScraperJournalsCache.MakeKey(_parserName, _parserType);
        string value = _id.ToString();
        await _database.StringSetAsync(key, value);
    }

    public ActiveScraperJournalCacheOutput Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    ) => new(_database, id, scraperName, scraperType);

    public ActiveScraperJournalCacheOutput(IDatabase database)
        : this(database, Guid.Empty, string.Empty, string.Empty) { }

    private ActiveScraperJournalCacheOutput(
        IDatabase database,
        Guid id,
        string parserName,
        string parserType
    )
    {
        _database = database;
        _id = id;
        _parserName = parserName;
        _parserType = parserType;
    }
}
