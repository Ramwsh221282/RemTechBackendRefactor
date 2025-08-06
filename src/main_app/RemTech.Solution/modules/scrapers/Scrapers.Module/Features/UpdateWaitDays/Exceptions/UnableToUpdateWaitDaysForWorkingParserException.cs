namespace Scrapers.Module.Features.UpdateWaitDays.Exceptions;

internal sealed class UnableToUpdateWaitDaysForWorkingParserException : Exception
{
    public UnableToUpdateWaitDaysForWorkingParserException(string parserName, string parserType)
        : base(
            $"Нельзя обновить время ожидания парсера {parserName} {parserType} в рабочем состоянии."
        ) { }

    public UnableToUpdateWaitDaysForWorkingParserException(
        string parserName,
        string parserType,
        Exception inner
    )
        : base(
            $"Нельзя обновить время ожидания парсера {parserName} {parserType} в рабочем состоянии.",
            inner
        ) { }
}
