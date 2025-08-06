using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Database;

internal interface IParsersWithNewLinkStorage
{
    Task<ParserWhereToPutLink> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task<ParserWithNewLink> Save(ParserWithNewLink parser, CancellationToken ct = default);
}
