namespace Scrapers.Module.Features.IncreaseProcessedAmount.Models;

internal sealed record ParserWithIncreasedProcessed(
    string ParserName,
    string ParserType,
    string ParserLinkName,
    int ParserProcessed,
    int LinkProcessed
);
