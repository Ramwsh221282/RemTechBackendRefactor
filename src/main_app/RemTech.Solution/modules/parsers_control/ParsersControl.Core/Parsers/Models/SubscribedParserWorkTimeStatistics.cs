using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed record SubscribedParserWorkTimeStatistics
{
    private const long InitialValue = 0;
    public long TotalElapsedSeconds { get; private init; }
    public int Hours => (int)(TotalElapsedSeconds / 3600);
    public int Minutes => (int)((TotalElapsedSeconds % 3600) / 60);
    public int Seconds => (int)(TotalElapsedSeconds % 60);

    private SubscribedParserWorkTimeStatistics(long totalElapsedSeconds)
    {
        TotalElapsedSeconds = totalElapsedSeconds;
    }
    
    public static SubscribedParserWorkTimeStatistics New()
    {
        return new SubscribedParserWorkTimeStatistics(InitialValue);
    }
    
    public static Result<SubscribedParserWorkTimeStatistics> FromTotalElapsedSeconds(long totalElapsedSeconds)
    {
        if (totalElapsedSeconds < InitialValue) 
            return Error.Validation("Общее время работы парсера не может быть отрицательным.");
        return new SubscribedParserWorkTimeStatistics(totalElapsedSeconds);        
    }
}