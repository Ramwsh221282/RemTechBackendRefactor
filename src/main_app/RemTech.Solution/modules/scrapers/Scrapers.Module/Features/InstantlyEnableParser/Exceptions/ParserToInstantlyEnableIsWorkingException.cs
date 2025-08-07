namespace Scrapers.Module.Features.InstantlyEnableParser.Exceptions;

internal sealed class ParserToInstantlyEnableIsWorkingException : Exception
{
    public ParserToInstantlyEnableIsWorkingException()
        : base("Парсер уже в рабочем состоянии") { }

    public ParserToInstantlyEnableIsWorkingException(Exception ex)
        : base("Парсер уже в рабочем состоянии", ex) { }
}
