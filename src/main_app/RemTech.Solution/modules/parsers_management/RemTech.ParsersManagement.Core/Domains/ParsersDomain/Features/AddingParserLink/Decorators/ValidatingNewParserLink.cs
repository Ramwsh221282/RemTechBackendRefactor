using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Decorators;

public sealed class ValidatingNewParserLink(INewParserLink inner) : INewParserLink
{
    public Status<IParserLink> Register(AddParserLink addLink)
    {
        return addLink.Errored() ? addLink.Error() : inner.Register(addLink);
    }
}
