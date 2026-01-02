using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public sealed class SubscribedParsersCollection(IEnumerable<SubscribedParser> parsers)
{
    private SubscribedParsersCollection() : this([]) { }
    
    private SubscribedParser[] Parsers { get; set; } = [..parsers];
    
    public bool IsEmpty() => Parsers.Length == 0;
    
    public static SubscribedParsersCollection Empty() => new();

    public Result<Unit> PermanentlyDisableAll()
    {
        if (IsEmpty()) return Error.NotFound($"Список парсеров пуст.");
        foreach (SubscribedParser parser in Parsers)
            parser.Disable(); 
        
        return Result.Success(Unit.Value);
    }
    
    public Result<Unit> PermanentlyEnableAll()
    {
        if (IsEmpty()) return Error.NotFound($"Список парсеров пуст.");
        foreach (SubscribedParser parser in Parsers)
        {
            Result<Unit> enabling = parser.PermantlyEnable();
            if (enabling.IsFailure) return enabling.Error;
        }
        
        return Result.Success(Unit.Value);
    }
    
    public IEnumerable<Guid> GetIdentifiers() => Parsers.Select(p => p.Id.Value);
    
    public IEnumerable<SubscribedParser> Read() => Parsers;
}