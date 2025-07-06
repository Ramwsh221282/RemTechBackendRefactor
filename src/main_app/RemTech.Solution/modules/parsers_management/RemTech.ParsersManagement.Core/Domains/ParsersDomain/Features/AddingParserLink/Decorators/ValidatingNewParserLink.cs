using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Decorators;

public sealed class ValidatingNewParserLink(INewParserLink inner) : INewParserLink
{
    public Status<IParserLink> Register(AddParserLink addLink) =>
        new ValidatingOperation(addLink).Process(() => inner.Register(addLink));
}
