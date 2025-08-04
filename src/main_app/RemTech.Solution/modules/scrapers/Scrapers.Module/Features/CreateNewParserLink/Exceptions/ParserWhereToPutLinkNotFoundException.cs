namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class ParserWhereToPutLinkNotFoundException : Exception
{
    public ParserWhereToPutLinkNotFoundException(string name, string type)
        : base($"Не удается найти парсер с названием {name} {type}") { }

    public ParserWhereToPutLinkNotFoundException(string name, string type, Exception inner)
        : base($"Не удается найти парсер с названием {name} {type}", inner) { }
}
