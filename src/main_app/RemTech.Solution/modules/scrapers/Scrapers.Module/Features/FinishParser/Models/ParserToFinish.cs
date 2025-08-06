namespace Scrapers.Module.Features.FinishParser.Models;

internal sealed record ParserToFinish(string ParserName, string ParserType, int WaitDays)
{
    public FinishedParser Finish(long totalElapsedSeconds)
    {
        int hours = CalculateHoursFromElapsedSeconds(totalElapsedSeconds);
        int minutes = CalculateMinutesFromElapsedSeconds(totalElapsedSeconds);
        int seconds = CalculateSecondsFromElapsedSeconds(totalElapsedSeconds);
        DateTime newLastRun = DateTime.UtcNow;
        DateTime newNextRun = newLastRun.AddDays(WaitDays);
        return new FinishedParser(
            ParserName,
            ParserType,
            "Ожидает",
            newLastRun,
            newNextRun,
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
