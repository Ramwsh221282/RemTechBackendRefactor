namespace Scrapers.Module.Features.InstantlyDisableParser.Exceptions;

internal sealed class CannotInstantlyDisableNotWorkingParserException : Exception
{
    public CannotInstantlyDisableNotWorkingParserException(string parserName, string parserType)
        : base(
            $"Не удается немедленно остановить парсер {parserName} {parserType}. Не в рабочем состоянии"
        ) { }

    public CannotInstantlyDisableNotWorkingParserException(
        string parserName,
        string parserType,
        Exception inner
    )
        : base(
            $"Не удается немедленно остановить парсер {parserName} {parserType}. Не в рабочем состоянии",
            inner
        ) { }
}
