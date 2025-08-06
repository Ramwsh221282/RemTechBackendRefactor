namespace Scrapers.Module.Features.StartParser.RabbitMq;

internal sealed record ParserLinkStartedRabbitMqMessage(
    string ParserName,
    string ParserType,
    string LinkName,
    string LinkUrl
);
