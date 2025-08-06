using Scrapers.Module.Features.FinishParserLink.Exceptions;

namespace Scrapers.Module.Features.FinishParserLink.Models;

internal sealed record ParserLinkToFinish(
    string ParserName,
    string ParserState,
    string ParserType,
    string LinkName
)
{
    public FinishedParserLink Finish(long totalElapsedSeconds)
    {
        if (ParserState != "Работает")
            throw new CannotFinishParserLinkFromNotWorkingParserException(ParserName, ParserType);
        int hours = CalculateHoursFromElapsedSeconds(totalElapsedSeconds);
        int minutes = CalculateMinutesFromElapsedSeconds(totalElapsedSeconds);
        int seconds = CalculateSecondsFromElapsedSeconds(totalElapsedSeconds);
        return new FinishedParserLink(
            ParserName,
            ParserType,
            LinkName,
            totalElapsedSeconds,
            seconds,
            minutes,
            hours
        );
    }

    private static int CalculateHoursFromElapsedSeconds(long seconds) => (int)(seconds / 3600);

    private static int CalculateMinutesFromElapsedSeconds(long seconds) =>
        (int)((seconds % 3600) / 60);

    private static int CalculateSecondsFromElapsedSeconds(long seconds) => (int)(seconds % 60);
}
