namespace Scrapers.Module.Features.IncreaseProcessedAmount.Exceptions;

internal sealed class UnableToFindIncreaseProcessedParserException : Exception
{
    public UnableToFindIncreaseProcessedParserException(
        string parserName,
        string parserType,
        string linkName
    )
        : base(
            $"Не удается найти парсер для увеличения количества обработанных объявлений. {parserName} {parserType} {linkName}"
        ) { }

    public UnableToFindIncreaseProcessedParserException(
        string parserName,
        string parserType,
        string linkName,
        Exception inner
    )
        : base(
            $"Не удается найти парсер для увеличения количества обработанных объявлений. {parserName} {parserType} {linkName}",
            inner
        ) { }
}
