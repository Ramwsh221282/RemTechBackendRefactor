using Scrapers.Module.Features.FinishParserLink.Models;

namespace Scrapers.Module.Features.FinishParserLink.Database;

internal interface IFinishedParserLinkStorage
{
    Task<ParserLinkToFinish> Fetch(
        string parserName,
        string linkName,
        string parserType,
        CancellationToken ct = default
    );
    Task<FinishedParserLink> Save(FinishedParserLink link, CancellationToken ct = default);
}
