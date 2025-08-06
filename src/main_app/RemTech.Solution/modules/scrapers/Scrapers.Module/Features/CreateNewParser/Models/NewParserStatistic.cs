namespace Scrapers.Module.Features.CreateNewParser.Models;

internal sealed record NewParserStatistic
{
    public int ProcessedAmount { get; }
    public long TotalElapsedSeconds { get; }
    public int ElapsedSeconds { get; }
    public int ElapsedHours { get; }
    public int ElapsedMinutes { get; }

    private NewParserStatistic(
        int processedAmount,
        int totalElapsedSeconds,
        int elapsedSeconds,
        int elapsedHours,
        int elapsedMinutes
    )
    {
        ProcessedAmount = processedAmount;
        TotalElapsedSeconds = totalElapsedSeconds;
        ElapsedSeconds = elapsedSeconds;
        ElapsedHours = elapsedHours;
        ElapsedMinutes = elapsedMinutes;
    }

    public static NewParserStatistic Create()
    {
        return new NewParserStatistic(0, 0, 0, 0, 0);
    }
}
