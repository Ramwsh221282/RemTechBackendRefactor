using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public readonly record struct SubscribedParserParsedCount
{
    public int Value { get; private init; }
    
    public SubscribedParserParsedCount() => Value = 0;
    
    private SubscribedParserParsedCount(int value) => Value = value;

    public Result<SubscribedParserParsedCount> Add(int amount)
    {
        if (amount < 0) return Error.Validation("Количество добавляемых данных парсером не может быть отрицательным.");
        return new SubscribedParserParsedCount(Value + amount);
    }
    
    public static Result<SubscribedParserParsedCount> Create(int value)
    {
        return value < 0
            ? Error.Validation("Количество обработанных данных парсером не может быть отрицательным.")
            : new SubscribedParserParsedCount(value);
    }

    public static SubscribedParserParsedCount New()
    {
        return Create(0).Value;
    }
}