namespace Scrapers.Module.Features.StartParser.RabbitMq;

internal sealed record ParserStartedRabbitMqMessage(
    string ParserName,
    string ParserType,
    string ParserDomain,
    IEnumerable<ParserLinkStartedRabbitMqMessage> Links
);
