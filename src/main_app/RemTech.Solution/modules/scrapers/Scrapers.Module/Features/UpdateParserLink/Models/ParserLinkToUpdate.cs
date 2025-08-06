using Scrapers.Module.Features.UpdateParserLink.Exceptions;

namespace Scrapers.Module.Features.UpdateParserLink.Models;

internal sealed class ParserLinkToUpdate
{
    private readonly string _parserName;
    private readonly string _parserType;
    private readonly string _parserDomain;
    private readonly string _linkUrl;
    private readonly string _linkName;

    public ParserLinkToUpdate(
        string parserName,
        string parserType,
        string parserDomain,
        string linkUrl,
        string linkName
    )
    {
        _parserName = parserName;
        _parserType = parserType;
        _parserDomain = parserDomain;
        _linkUrl = linkUrl;
        _linkName = linkName;
    }

    public UpdatedParserLink Update(string linkUrl, string linkName)
    {
        if (string.IsNullOrWhiteSpace(linkUrl))
            throw new ParserLinkUpdateLinkUrlException();
        if (string.IsNullOrWhiteSpace(linkName))
            throw new ParserLinkUpdateNameEmptyException();
        return !linkUrl.Contains(_parserDomain)
            ? throw new ParserLinkUpdateDomainMisatchException(_parserDomain)
            : new UpdatedParserLink(
                _linkName,
                _linkUrl,
                _parserName,
                _parserType,
                linkUrl,
                linkName
            );
    }
}
