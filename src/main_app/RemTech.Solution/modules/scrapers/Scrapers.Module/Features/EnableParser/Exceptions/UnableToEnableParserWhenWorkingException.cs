namespace Scrapers.Module.Features.EnableParser.Exceptions;

internal sealed class UnableToEnableParserWhenWorkingException : Exception
{
    public UnableToEnableParserWhenWorkingException()
        : base($"Нельзя включить парсер который уже работает.") { }

    public UnableToEnableParserWhenWorkingException(Exception inner)
        : base($"Нельзя включить парсер который уже работает.", inner) { }
}
