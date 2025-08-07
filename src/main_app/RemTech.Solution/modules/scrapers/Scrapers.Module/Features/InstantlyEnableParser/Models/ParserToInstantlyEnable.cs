using Scrapers.Module.Features.InstantlyEnableParser.Exceptions;
using Scrapers.Module.Features.StartParser.Models;
using Scrapers.Module.Features.StartParser.RabbitMq;

namespace Scrapers.Module.Features.InstantlyEnableParser.Models;

internal sealed class ParserToInstantlyEnable(string name, string type, string state, string domain)
{
    private readonly LinkedList<ParserLinkToInstantlyEnable> _links = [];

    public void AddLink(string name1, string linkUrl, string linkParserName, string linkParserType)
    {
        if (name != linkParserName && type != linkParserType)
            return;
        ParserLinkToInstantlyEnable link = new ParserLinkToInstantlyEnable(
            name1,
            linkUrl,
            type,
            name
        );
        _links.AddLast(link);
    }

    public InstantlyEnabledParser Enable()
    {
        if (_links.Count == 0)
            throw new UnableToInstantlyStartParserWithoutLinksException();
        return state == "Работает"
            ? throw new ParserToInstantlyEnableIsWorkingException()
            : new InstantlyEnabledParser(name, type);
    }

    public async Task PublishStarted(IParserStartedPublisher publisher)
    {
        StartedParser started = new StartedParser(
            name,
            type,
            domain,
            state,
            0,
            _links.Select(l => l.StartedLink()).ToHashSet()
        );
        await publisher.Publish(started);
    }
}
