using Scrapers.Module.Features.UpdateParserLink.Models;

namespace Scrapers.Module.Features.UpdateParserLink.Database;

internal interface IParserLinkToUpdateStorage
{
    Task<ParserLinkToUpdate> Fetch(
        string parserName,
        string parserType,
        string linkName,
        string linkUrl,
        CancellationToken ct = default
    );

    Task Save(
        string parserName,
        string parserType,
        string linkName,
        string linkUrl,
        string oldLinkName,
        string oldLinkUrl,
        CancellationToken ct = default
    );
}
