namespace Scrapers.Module.Features.CreateNewParserLink.Models;

internal sealed record NewParserLink
{
    public string Name { get; }
    public string ParserName { get; }
    public string ParserType { get; }
    public string Url { get; }
    public bool Active { get; }
    public NewParserLinkStatistics Statistics { get; }

    private NewParserLink(
        string name,
        string parserName,
        string parserType,
        string url,
        bool active,
        NewParserLinkStatistics statistics
    )
    {
        Name = name;
        ParserType = parserType;
        ParserName = parserName;
        Url = url;
        Active = active;
        Statistics = statistics;
    }

    public static NewParserLink Create(string name, string url, ParserWhereToPutLink parser)
    {
        NewParserLinkStatistics statistics = NewParserLinkStatistics.Create();
        return new NewParserLink(name, parser.Name, parser.Type, url, false, statistics);
    }
}
