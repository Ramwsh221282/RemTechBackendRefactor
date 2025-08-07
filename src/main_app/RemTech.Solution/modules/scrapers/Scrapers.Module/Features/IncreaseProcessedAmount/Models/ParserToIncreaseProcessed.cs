namespace Scrapers.Module.Features.IncreaseProcessedAmount.Models;

internal sealed record ParserToIncreaseProcessed(
    string ParserName,
    string ParserType,
    string ParserLinkName,
    int ParserProcessed,
    int LinkProcessed
)
{
    public ParserWithIncreasedProcessed Increase()
    {
        int newParserProcessed = ParserProcessed + 1;
        int newLinkProcessed = LinkProcessed + 1;
        return new ParserWithIncreasedProcessed(
            ParserName,
            ParserType,
            ParserLinkName,
            newParserProcessed,
            newLinkProcessed
        );
    }
}
