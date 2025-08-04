using Scrapers.Module.Features.CreateNewParserLink.Models;

namespace Scrapers.Module.Features.CreateNewParserLink.Exceptions;

internal sealed class ParserLinkUrlAlreadyExistsException : Exception
{
    public ParserLinkUrlAlreadyExistsException(NewParserLink link)
        : base($"Ссылка {link.Url} уже существует.") { }

    public ParserLinkUrlAlreadyExistsException(NewParserLink link, Exception inner)
        : base($"Ссылка {link.Url} уже существует.", inner) { }
}
