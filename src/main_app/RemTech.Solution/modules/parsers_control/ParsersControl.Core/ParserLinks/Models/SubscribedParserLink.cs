using ParsersControl.Core.Parsers.Models;

namespace ParsersControl.Core.ParserLinks.Models;

public sealed class SubscribedParserLink
{
    private SubscribedParserLink(
        SubscribedParserId parserId, 
        SubscribedParserLinkId id, 
        SubscribedParserLinkUrlInfo urlInfo, 
        bool active) => 
        (ParserId, Id, UrlInfo, Active) = (parserId, id, urlInfo, active);
    
    private SubscribedParserLink(SubscribedParserId parserId, SubscribedParserLinkUrlInfo urlInfo)
    : this(parserId, SubscribedParserLinkId.New(), urlInfo, false) { }
    
    private SubscribedParserLink(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo)
        : this(parser.Id, SubscribedParserLinkId.New(), urlInfo, false) { }
    
    public SubscribedParserId ParserId { get; init; }
    public SubscribedParserLinkId Id { get; init; }
    public SubscribedParserLinkUrlInfo UrlInfo { get; init; }
    public bool Active { get; init; }
    
    public static SubscribedParserLink New(SubscribedParser parser, SubscribedParserLinkUrlInfo urlInfo) =>
        new(parser, urlInfo);
}