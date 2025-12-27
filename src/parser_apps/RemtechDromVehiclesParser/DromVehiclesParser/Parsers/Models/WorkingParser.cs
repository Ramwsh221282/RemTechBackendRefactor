namespace DromVehiclesParser.Parsers.Models;

public sealed record WorkingParser(Guid Id, string Domain, string Type, DateTime StartDateTime, DateTime? EndDateTime)
{
    public WorkingParser Finish()
    {
        return this with { EndDateTime = DateTime.UtcNow };
    }

    public long TotalElapsedInSeconds()
    {
        TimeSpan difference = EndDateTime!.Value - StartDateTime;
        return (long)difference.TotalSeconds;
    }
}