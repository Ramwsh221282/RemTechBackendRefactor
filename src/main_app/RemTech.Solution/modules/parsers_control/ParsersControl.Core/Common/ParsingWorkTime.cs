using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Common;

public sealed record ParsingWorkTime
{
    private const long InitialValue = 0;
    public long TotalElapsedSeconds { get; private init; }
    public int Hours => (int)(TotalElapsedSeconds / 3600);
    public int Minutes => (int)((TotalElapsedSeconds % 3600) / 60);
    public int Seconds => (int)(TotalElapsedSeconds % 60);

    private ParsingWorkTime(long totalElapsedSeconds)
    {
        TotalElapsedSeconds = totalElapsedSeconds;
    }
    
    public static ParsingWorkTime New()
    {
        return new ParsingWorkTime(InitialValue);
    }
    
    public static Result<ParsingWorkTime> FromTotalElapsedSeconds(long totalElapsedSeconds)
    {
        if (totalElapsedSeconds < InitialValue) 
            return Error.Validation("Общее время работы парсера не может быть отрицательным.");
        return new ParsingWorkTime(totalElapsedSeconds);        
    }
}