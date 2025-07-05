using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity;

public interface IChangedLinkActivity
{
    public Status<IParserLink> Changed(ChangeLinkActivity change);
}