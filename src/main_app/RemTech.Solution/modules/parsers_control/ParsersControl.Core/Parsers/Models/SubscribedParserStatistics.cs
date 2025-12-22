using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed record SubscribedParserStatistics
{
    public SubscribedParserStatistics(
        SubscribedParserWorkTimeStatistics workTime, 
        SubscribedParserParsedCount parsedCount)
    {
        WorkTime = workTime;
        ParsedCount = parsedCount;        
    }
    
    public SubscribedParserWorkTimeStatistics WorkTime { get; private init; }
    public SubscribedParserParsedCount ParsedCount { get; private init; }

    public Result<SubscribedParserStatistics> IncreaseParsedCount(int amount)
    {
        Result<SubscribedParserParsedCount> updated = ParsedCount.Add(amount);
        if (updated.IsFailure) return updated.Error;
        return this with { ParsedCount = updated.Value };
    }
    
    public Result<SubscribedParserStatistics> AddWorkTime(long totalElapsedSeconds)
    {
        Result<SubscribedParserWorkTimeStatistics> updated = SubscribedParserWorkTimeStatistics.FromTotalElapsedSeconds(totalElapsedSeconds);
        if (updated.IsFailure) return updated.Error;
        return this with { WorkTime = updated.Value };
    }

    public SubscribedParserStatistics ResetWorkTime()
    {
        return this with { WorkTime = SubscribedParserWorkTimeStatistics.New() };
    }
    
    public SubscribedParserStatistics ResetParsedCount()
    {
        return this with { ParsedCount = SubscribedParserParsedCount.New() };
    }
    
    public static SubscribedParserStatistics New()
    {
        SubscribedParserWorkTimeStatistics workTime = SubscribedParserWorkTimeStatistics.New();
        SubscribedParserParsedCount parsedCount = SubscribedParserParsedCount.New();
        return new SubscribedParserStatistics(workTime, parsedCount);
    }
}