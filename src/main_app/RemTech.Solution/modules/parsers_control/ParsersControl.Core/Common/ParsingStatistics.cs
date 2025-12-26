using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

public sealed record ParsingStatistics
{
    public ParsingStatistics(
        ParsingWorkTime workTime, 
        ParsedCount parsedCount)
    {
        WorkTime = workTime;
        ParsedCount = parsedCount;        
    }
    
    public ParsingWorkTime WorkTime { get; private init; }
    public ParsedCount ParsedCount { get; private init; }

    public Result<ParsingStatistics> IncreaseParsedCount(int amount)
    {
        Result<ParsedCount> updated = ParsedCount.Add(amount);
        if (updated.IsFailure) return updated.Error;
        return this with { ParsedCount = updated.Value };
    }
    
    public Result<ParsingStatistics> AddWorkTime(long totalElapsedSeconds)
    {
        Result<ParsingWorkTime> updated = ParsingWorkTime.FromTotalElapsedSeconds(totalElapsedSeconds);
        if (updated.IsFailure) return updated.Error;
        return this with { WorkTime = updated.Value };
    }

    public ParsingStatistics ResetWorkTime()
    {
        return this with { WorkTime = ParsingWorkTime.New() };
    }
    
    public ParsingStatistics ResetParsedCount()
    {
        return this with { ParsedCount = ParsedCount.New() };
    }
    
    public static ParsingStatistics New()
    {
        ParsingWorkTime workTime = ParsingWorkTime.New();
        ParsedCount parsedCount = ParsedCount.New();
        return new ParsingStatistics(workTime, parsedCount);
    }
}