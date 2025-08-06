namespace Parsing.RabbitMq.StartParsing;

public sealed record ParserStartedRabbitMqMessage(
    string ParserName,
    string ParserType,
    string ParserDomain,
    IEnumerable<ParserLinkStartedRabbitMqMessage> Links
);
