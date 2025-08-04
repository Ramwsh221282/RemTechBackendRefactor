namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class ParserWhereToPutLinkUnsupportedTypeException : Exception
{
    public ParserWhereToPutLinkUnsupportedTypeException(string type)
        : base($"Тип {type} у парсеров не поддерживается.") { }
}
