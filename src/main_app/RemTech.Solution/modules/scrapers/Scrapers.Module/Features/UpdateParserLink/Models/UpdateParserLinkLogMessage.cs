namespace Scrapers.Module.Features.UpdateParserLink.Models;

internal sealed class UpdateParserLinkLogMessage
{
    private readonly string _parserName;
    private readonly string _parserType;
    private readonly string _linkUrl;
    private readonly string _linkName;

    public UpdateParserLinkLogMessage(
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
    }

    public void Log(Serilog.ILogger logger)
    {
        logger.Information(
            "Изменена ссылка {ParserName} {ParserType} {LinkUrl} {LinkName}",
            _parserName,
            _parserType,
            _linkUrl,
            _linkName
        );
    }
}
