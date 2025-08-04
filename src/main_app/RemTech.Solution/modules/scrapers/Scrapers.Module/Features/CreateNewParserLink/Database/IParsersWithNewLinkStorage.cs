using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Database;

internal interface IParsersWithNewLinkStorage
{
    Task<ParserWithNewLink> Save(ParserWithNewLink parser, CancellationToken ct = default);
}
