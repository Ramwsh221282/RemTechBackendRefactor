namespace Scrapers.Module.Features.UpdateWaitDays.Endpoint;

internal sealed record ParserWithUpdatedWaitDays(
    string ParserName,
    string ParserType,
    string ParserState,
    DateTime NextRun,
    int WaitDays
);
