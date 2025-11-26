namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public sealed record Unit
{
    public static Unit Value { get; } = new();
    
    private Unit()
    {
        
    }

    public static Unit<T> UnitOf<T>(T value)
    {
        return  new Unit<T>(value);
    }
}

public sealed record Unit<T>
{
    private readonly T _value;

    internal Unit(T value)
    {
        _value = value;
    }

    public async Task<Result<Unit>> Executed(Func<T, Task> operation)
    {
        await  operation(_value);
        return Unit.Value;
    }
}

public static class UnitModule
{
    extension(Unit)
    {
        public static async Task<Result<Unit>> Executed(Func<Task> operation)
        {
            await operation();
            return Unit.Value;
        }
    }
}