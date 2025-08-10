namespace RemTech.Spares.Module.Features.SinkSpare;

internal sealed record ParserLinkBody(
    string ParserName,
    string ParserType,
    string ParserDomain,
    string LinkName,
    string LinkUrl
);
