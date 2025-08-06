namespace Scrapers.Module.Features.FinishParser.Exceptions;

internal sealed class CannotFinishNotWorkingParserException : Exception
{
    public CannotFinishNotWorkingParserException(string parserName, string parserType)
        : base(
            $"Не удается завершить работу парсера {parserName} {parserType}. Не в рабочем состоянии."
        ) { }

    public CannotFinishNotWorkingParserException(
        string parserName,
        string parserType,
        Exception inner
    )
        : base(
            $"Не удается завершить работу парсера {parserName} {parserType}. Не в рабочем состоянии.",
            inner
        ) { }
}
