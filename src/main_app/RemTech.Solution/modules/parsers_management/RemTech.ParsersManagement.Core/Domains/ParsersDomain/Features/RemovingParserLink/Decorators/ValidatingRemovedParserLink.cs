using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Decorators;

public sealed class ValidatingRemovedParserLink(IRemovedParserLink inner) : IRemovedParserLink
{
    public Status<IParserLink> Removed(RemoveParserLink remove)
    {
        if (remove.Errored())
            return remove.Error();
        if (remove.TakeOwner().WorkState().AtWork())
            return new EditParserWhenWorkingError();
        return inner.Removed(remove);
    }
}
