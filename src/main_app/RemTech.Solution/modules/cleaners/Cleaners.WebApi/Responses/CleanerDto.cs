namespace Cleaners.WebApi.Responses;

public sealed class CleanerDto
{
    public required Guid Id { get; init; }
    public required int Threshold { get; init; }
    public required int ProcessedAmount { get; init; }
    public required DateTime? LastRun { get; init; }
    public required DateTime? NextRun { get; init; }
    public required int WaitDays { get; init; }
    public required long ElapsedHours { get; init; }
    public required long ElapsedMinutes { get; init; }
    public required long ElapsedSeconds { get; init; }
    public required string State { get; init; }
}
