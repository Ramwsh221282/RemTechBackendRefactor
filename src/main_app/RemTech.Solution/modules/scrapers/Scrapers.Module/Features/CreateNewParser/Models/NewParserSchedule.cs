namespace Scrapers.Module.Features.CreateNewParser.Models;

internal sealed record NewParserSchedule
{
    public DateTime LastRun { get; }
    public DateTime NextRun { get; }
    public int WaitDays { get; }

    private NewParserSchedule(DateTime lastRun, DateTime nextRun, int waitDays)
    {
        LastRun = lastRun;
        NextRun = nextRun;
        WaitDays = waitDays;
    }

    public static NewParserSchedule Create()
    {
        DateTime utcNow = DateTime.UtcNow;
        int waitDays = 1;
        return new NewParserSchedule(utcNow, utcNow, waitDays);
    }
}
