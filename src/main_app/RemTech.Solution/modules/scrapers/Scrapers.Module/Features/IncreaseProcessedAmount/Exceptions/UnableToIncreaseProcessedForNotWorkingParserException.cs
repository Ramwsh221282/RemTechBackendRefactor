namespace Scrapers.Module.Features.IncreaseProcessedAmount.Exceptions;

internal sealed class UnableToIncreaseProcessedForNotWorkingParserException : Exception
{
    public UnableToIncreaseProcessedForNotWorkingParserException(
        string parserName,
        string parserType
    )
        : base(
            $"Парсер {parserName} {parserType} должен быть в рабочем состоянии для увеличения количества обработанных объявлений."
        ) { }

    public UnableToIncreaseProcessedForNotWorkingParserException(
        string parserName,
        string parserType,
        Exception inner
    )
        : base(
            $"Парсер {parserName} {parserType} должен быть в рабочем состоянии для увеличения количества обработанных объявлений.",
            inner
        ) { }
}
