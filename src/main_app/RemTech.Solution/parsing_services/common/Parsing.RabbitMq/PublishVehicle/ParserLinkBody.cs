namespace Parsing.RabbitMq.PublishVehicle;

public sealed record ParserLinkBody(
    string ParserName,
    string ParserType,
    string ParserDomain,
    string LinkName,
    string LinkUrl
);
