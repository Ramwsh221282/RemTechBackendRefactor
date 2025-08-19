using Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;
using Scrapers.Module.Features.StartParser.Database;
using Scrapers.Module.Features.StartParser.RabbitMq;

namespace Scrapers.Module.Features.StartParser.Models;

public sealed record StartedParser(
    string ParserName,
    string ParserType,
    string ParserDomain,
    string ParserState,
    int Processed,
    HashSet<StartedParserLink> Links
)
{
    public Task Save(IParsersToStartStorage storage, CancellationToken ct = default)
    {
        return storage.Save(this, ct);
    }

    public Task Publish(IParserStartedPublisher publisher)
    {
        return publisher.Publish(this);
    }

    internal CreateScraperJournalCommand CreateJournalCommand()
    {
        return new CreateScraperJournalCommand(ParserName, ParserType);
    }
}
