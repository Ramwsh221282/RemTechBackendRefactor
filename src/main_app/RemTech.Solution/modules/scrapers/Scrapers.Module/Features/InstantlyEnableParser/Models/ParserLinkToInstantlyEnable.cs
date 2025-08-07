using Scrapers.Module.Features.StartParser.Models;

namespace Scrapers.Module.Features.InstantlyEnableParser.Models;

internal sealed class ParserLinkToInstantlyEnable
{
    private readonly string _name;
    private readonly string _linkUrl;
    private readonly string _parserType;
    private readonly string _parserName;

    public StartedParserLink StartedLink()
    {
        return new StartedParserLink(_name, _linkUrl, _parserName);
    }

    public ParserLinkToInstantlyEnable(
        string name,
        string linkUrl,
        string parserType,
        string parserName
    )
    {
        _name = name;
        _linkUrl = linkUrl;
        _parserType = parserType;
        _parserName = parserName;
    }
}
