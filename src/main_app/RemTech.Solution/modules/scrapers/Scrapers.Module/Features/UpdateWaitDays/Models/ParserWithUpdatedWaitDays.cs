namespace Scrapers.Module.Features.UpdateWaitDays.Models;

internal sealed record ParserWithUpdatedWaitDays(
    string ParserName,
    string ParserType,
    string ParserState,
    DateTime NextRun,
    int WaitDays
);
