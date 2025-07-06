using RemTech.ParsersManagement.Core.Common.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Decorators;

public sealed class ValidatingFinishedParserLink(IFinishedParserLink inner) : IFinishedParserLink
{
    public Status<IParserLink> Finished(FinishParserLink finish) =>
        new ValidatingOperation(finish).Process(() => inner.Finished(finish));
}
