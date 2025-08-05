using Scrapers.Module.Features.FinishParser.Models;

namespace Scrapers.Module.Features.FinishParser.Database;

internal interface IParserToFinishStorage
{
    Task<ParserToFinish> Fetch(
        string parserName,
        string parserType,
        CancellationToken ct = default
    );
    Task<FinishedParser> Save(FinishedParser parser, CancellationToken ct = default);
}
