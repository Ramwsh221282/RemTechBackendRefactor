namespace AvitoSparesParser.ParserStartConfiguration;

public sealed record ProcessingParser(
    Guid Id,
    string Domain,
    string Type,
    DateTime Entered,
    DateTime? Finished
)
{
    public ProcessingParser Finish()
    {
        return this with { Finished = DateTime.UtcNow };
    }
    
    public long TotalElapsedSeconds()
    {
        if (Finished is null) return 0;
        TimeSpan difference = Finished.Value - Entered;
        return (long)difference.TotalSeconds;
    }
}
