using Scrapers.Module.Features.RemovingParserLink.Models;

namespace Scrapers.Module.Features.RemovingParserLink.Database;

internal interface IRemovedParserLinksStorage
{
    Task<ParserLinkToRemove> Fetch(
        string linkName,
        string parserName,
        string parserType,
        CancellationToken ct = default
    );

    Task<RemovedParserLink> Save(RemovedParserLink parserLink, CancellationToken ct = default);
}
