using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink;

public interface INewParserLink
{
    Status<IParserLink> Register(AddParserLink addLink);
}
