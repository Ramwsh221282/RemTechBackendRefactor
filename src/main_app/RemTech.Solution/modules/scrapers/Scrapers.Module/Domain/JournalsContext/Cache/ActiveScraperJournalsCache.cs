using Scrapers.Module.Domain.JournalsContext.Exceptions;
using StackExchange.Redis;

namespace Scrapers.Module.Domain.JournalsContext.Cache;

internal sealed class ActiveScraperJournalsCache(ConnectionMultiplexer multiplexer)
{
    private const string KeyFormat = "{0}_{1}_journal";

    public async Task<Guid> GetIdentifier(string parserName, string parserType)
    {
        IDatabase database = multiplexer.GetDatabase();
        string key = MakeKey(parserName, parserType);
        string? journalId = await database.StringGetAsync(key);
        if (string.IsNullOrEmpty(journalId))
            throw new ScraperJournalByIdNotFoundException();
        return Guid.Parse(journalId);
    }

    public async Task RemoveIdentifier(string parserName, string parserType)
    {
        IDatabase database = multiplexer.GetDatabase();
        string key = MakeKey(parserName, parserType);
        await database.KeyDeleteAsync(key);
    }

    public async Task SaveIdentifier(ScraperJournal journal)
    {
        IDatabase database = multiplexer.GetDatabase();
        ActiveScraperJournalCacheOutput output = new(database);
        output = journal.PrintTo(output);
        await output.BehaveAsync();
    }

    public static string MakeKey(string parserName, string parserType)
    {
        string key = string.Format(KeyFormat, parserName, parserType);
        return key;
    }
}
