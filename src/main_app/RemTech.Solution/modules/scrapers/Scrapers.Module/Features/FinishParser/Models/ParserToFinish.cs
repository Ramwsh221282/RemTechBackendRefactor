using Scrapers.Module.Features.FinishParser.Exceptions;

namespace Scrapers.Module.Features.FinishParser.Models;

internal sealed record ParserToFinish(string ParserName, string ParserType, string ParserState)
{
    public FinishedParser Finish(long totalElapsedSeconds)
    {
        if (ParserState != "Работает")
            throw new CannotFinishNotWorkingParserException(ParserName, ParserType);
        int hours = CalculateHoursFromElapsedSeconds(totalElapsedSeconds);
        int minutes = CalculateMinutesFromElapsedSeconds(totalElapsedSeconds);
        int seconds = CalculateSecondsFromElapsedSeconds(totalElapsedSeconds);
        return new FinishedParser(
            ParserName,
            ParserType,
            totalElapsedSeconds,
            seconds,
            hours,
            minutes
        );
    }

    private static int CalculateHoursFromElapsedSeconds(long seconds) => (int)(seconds / 3600);

    private static int CalculateMinutesFromElapsedSeconds(long seconds) =>
        (int)((seconds % 3600) / 60);

    private static int CalculateSecondsFromElapsedSeconds(long seconds) => (int)(seconds % 60);
}
