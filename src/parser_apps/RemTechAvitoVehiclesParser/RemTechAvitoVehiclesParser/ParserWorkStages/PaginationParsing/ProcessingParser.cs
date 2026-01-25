namespace RemTechAvitoVehiclesParser.ParserWorkStages.PaginationParsing;

public sealed record ProcessingParser(
    Guid Id,
    string Domain,
    string Type,
    DateTime StartDateTime,
    DateTime? EndDateTime)
{
    public ProcessingParser Finish() => this with { EndDateTime = DateTime.UtcNow };
    public bool CanCalculateTotalElapsedSeconds() => EndDateTime.HasValue;
    public long CalculateTotalElapsedSeconds()
    {
        if (!CanCalculateTotalElapsedSeconds())
            throw new InvalidOperationException("Cannot calculate elapsed seconds for a parser that has not ended.");
        TimeSpan difference = EndDateTime!.Value - StartDateTime;
        return (long)difference.TotalSeconds;
    }
}
