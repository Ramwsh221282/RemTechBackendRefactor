using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink;

public interface IRemovedParserLink
{
    Status<IParserLink> Removed(RemoveParserLink remove);
}