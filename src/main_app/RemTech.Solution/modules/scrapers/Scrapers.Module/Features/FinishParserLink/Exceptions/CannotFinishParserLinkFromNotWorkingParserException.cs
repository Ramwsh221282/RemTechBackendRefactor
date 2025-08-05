namespace Scrapers.Module.Features.FinishParserLink.Exceptions;

internal sealed class CannotFinishParserLinkFromNotWorkingParserException : Exception
{
    public CannotFinishParserLinkFromNotWorkingParserException(string parserName, string parserType)
        : base(
            $"Для завершения работы ссылки {parserName} {parserType} должен быть в рабочем состоянии."
        ) { }

    public CannotFinishParserLinkFromNotWorkingParserException(
        string parserName,
        string parserType,
        Exception inner
    )
        : base(
            $"Для завершения работы ссылки {parserName} {parserType} должен быть в рабочем состоянии.",
            inner
        ) { }
}
