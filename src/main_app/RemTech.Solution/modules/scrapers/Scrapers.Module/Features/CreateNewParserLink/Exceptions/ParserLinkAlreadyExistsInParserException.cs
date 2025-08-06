using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class ParserLinkAlreadyExistsInParserException : Exception
{
    public ParserLinkAlreadyExistsInParserException(ParserWhereToPutLink parser, NewParserLink link)
        : base($"Ссылка {link.Name} уже существует в парсере {parser.Name}") { }

    public ParserLinkAlreadyExistsInParserException(
        ParserWhereToPutLink parser,
        NewParserLink link,
        Exception inner
    )
        : base($"Ссылка {link.Name} уже существует в парсере {parser.Name}", inner) { }
}
