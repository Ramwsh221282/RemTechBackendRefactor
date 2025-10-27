namespace Cleaner.WebApi.Models;

public sealed class WorkingCleaner
{
    public required Guid Id { get; set; }
    public required int CleanedAmount { get; set; }
    public required string State { get; set; }
    public required long TotalElapsedSeconds { get; set; }
    public required int ItemsDateDayThreshold { get; set; }
}
