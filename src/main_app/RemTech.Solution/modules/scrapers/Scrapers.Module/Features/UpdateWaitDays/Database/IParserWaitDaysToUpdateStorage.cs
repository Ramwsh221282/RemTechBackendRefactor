using Scrapers.Module.Features.UpdateWaitDays.Endpoint;

namespace Scrapers.Module.Features.UpdateWaitDays.Database;

internal interface IParserWaitDaysToUpdateStorage
{
    Task<ParserWaitDaysToUpdate> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task<ParserWithUpdatedWaitDays> Save(
        ParserWithUpdatedWaitDays parser,
        CancellationToken ct = default
    );
}
