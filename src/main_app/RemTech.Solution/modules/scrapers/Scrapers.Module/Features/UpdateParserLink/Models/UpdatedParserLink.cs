using Scrapers.Module.Features.UpdateParserLink.Database;

namespace Scrapers.Module.Features.UpdateParserLink.Models;

internal sealed class UpdatedParserLink
{
    private readonly string _parserName;
    private readonly string _parserType;
    private readonly string _linkUrl;
    private readonly string _linkName;
    private readonly string _oldLinkName;
    private readonly string _oldLinkUrl;

    public UpdatedParserLink(
        string oldLinkName,
        string oldLinkUrl,
        string parserName,
        string parserType,
        string linkUrl,
        string linkName
    )
    {
        _parserName = parserName;
        _parserType = parserType;
        _linkUrl = linkUrl;
        _linkName = linkName;
        _oldLinkName = oldLinkName;
        _oldLinkUrl = oldLinkUrl;
    }

    public UpdateParserLinkLogMessage LogMessage()
    {
        return new UpdateParserLinkLogMessage(_parserName, _parserType, _linkUrl, _linkName);
    }

    public async Task Save(IParserLinkToUpdateStorage storage, CancellationToken ct = default)
    {
        await storage.Save(
            _parserName,
            _parserType,
            _linkName,
            _linkUrl,
            _oldLinkName,
            _oldLinkUrl,
            ct
        );
    }
}
