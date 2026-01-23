using ParsersControl.Core.ParserLinks.Models;

namespace ParsersControl.Infrastructure.Parsers.Queries.GetParsers;

public sealed record ParserLinkResponse(Guid Id, bool IsActive, string UrlName, string UrlValue)
{
    public static ParserLinkResponse Create(SubscribedParserLink link) =>
        new(link.Id.Value, link.Active, link.UrlInfo.Name, link.UrlInfo.Url);
}
