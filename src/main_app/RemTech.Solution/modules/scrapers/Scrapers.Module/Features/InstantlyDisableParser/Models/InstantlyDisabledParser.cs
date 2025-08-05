namespace Scrapers.Module.Features.InstantlyDisableParser.Models;

internal sealed record InstantlyDisabledParser(
    string ParserName,
    string ParserType,
    string ParserState
);
