using Scrapers.Module.Features.IncreaseProcessedAmount.Models;

namespace Scrapers.Module.Features.IncreaseProcessedAmount.Database;

internal interface IParserToIncreaseStorage
{
    Task<ParserToIncreaseProcessed> Fetch(
        string parserName,
        string parserType,
        string linkName,
        CancellationToken ct = default
    );
    Task<ParserWithIncreasedProcessed> Save(
        ParserWithIncreasedProcessed parser,
        CancellationToken ct = default
    );
}
