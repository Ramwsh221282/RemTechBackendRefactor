namespace Parsing.RabbitMq.StartParsing;

public sealed record ParserLinkStartedRabbitMqMessage(
    string ParserName,
    string ParserType,
    string LinkName,
    string LinkUrl
);
