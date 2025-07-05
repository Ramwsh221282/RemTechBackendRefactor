using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink;

public interface IFinishedParserLink
{
    public Status<IParserLink> Finished(FinishParserLink finish);
}