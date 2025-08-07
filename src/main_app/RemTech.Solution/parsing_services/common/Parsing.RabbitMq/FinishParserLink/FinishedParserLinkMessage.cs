namespace Parsing.RabbitMq.FinishParserLink;

public sealed record FinishedParserLinkMessage(
    string ParserName,
    string ParserType,
    string LinkName,
    long TotalElapsedSeconds
);
