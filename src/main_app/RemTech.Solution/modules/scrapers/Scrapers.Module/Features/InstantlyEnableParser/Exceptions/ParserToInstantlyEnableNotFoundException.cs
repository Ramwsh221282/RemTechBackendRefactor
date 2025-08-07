namespace Scrapers.Module.Features.InstantlyEnableParser.Exceptions;

internal sealed class ParserToInstantlyEnableNotFoundException : Exception
{
    public ParserToInstantlyEnableNotFoundException(string name, string type)
        : base($"Парсер {name} {type} не найден.") { }

    public ParserToInstantlyEnableNotFoundException(string name, string type, Exception ex)
        : base($"Парсер {name} {type} не найден.", ex) { }
}
