using ParsersControl.Core.Common;
using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Core.ParserLinks.Models;

public sealed class SubscribedParserLink
{
    private SubscribedParserLink(
        SubscribedParserId parserId, 
        SubscribedParserLinkId id, 
        SubscribedParserLinkUrlInfo urlInfo, 
        ParsingStatistics statistics, 
        bool active) => 
        (ParserId, Id, UrlInfo, Active, Statistics) = (parserId, id, urlInfo, active, statistics);

    private SubscribedParserLink(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo)
        : this(parser.Id, SubscribedParserLinkId.New(), urlInfo, ParsingStatistics.New(), false) { }
    public SubscribedParserId ParserId { get; init; }
    public SubscribedParserLinkId Id { get; init; }
    public SubscribedParserLinkUrlInfo UrlInfo { get; init; }
    public ParsingStatistics Statistics { get; init; }
    public bool Active { get; init; }
    public static SubscribedParserLink New(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo) =>
        new(parser, urlInfo);

    public static SubscribedParserLink Create(
        SubscribedParserId parserId,
        SubscribedParserLinkId id,
        SubscribedParserLinkUrlInfo urlInfo,
        ParsingStatistics statistics,
        bool active
    ) => new(parserId, id, urlInfo, statistics, active);
}