namespace Scrapers.Module.Features.IncreaseProcessedAmount.MessageBus;

public sealed record IncreaseProcessedMessage(
    string ParserName,
    string ParserType,
    string LinkName
);
