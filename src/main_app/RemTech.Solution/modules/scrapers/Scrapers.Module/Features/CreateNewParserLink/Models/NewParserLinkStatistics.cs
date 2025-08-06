namespace Scrapers.Module.Features.CreateNewParserLink.Models;

internal sealed record NewParserLinkStatistics
{
    public int ParsedAmount { get; }
    public long TotalElapsedSeconds { get; }
    public int ElapsedHours { get; }
    public int ElapsedMinutes { get; }
    public int ElapsedSeconds { get; }

    private NewParserLinkStatistics(
        int parsedAmount,
        int totalElapsedSeconds,
        int elapsedHours,
        int elapsedMinutes,
        int elapsedSeconds
    )
    {
        ParsedAmount = parsedAmount;
        TotalElapsedSeconds = totalElapsedSeconds;
        ElapsedHours = elapsedHours;
        ElapsedMinutes = elapsedMinutes;
        ElapsedSeconds = elapsedSeconds;
    }

    public static NewParserLinkStatistics Create()
    {
        return new NewParserLinkStatistics(0, 0, 0, 0, 0);
    }
}
