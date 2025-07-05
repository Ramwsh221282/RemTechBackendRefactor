using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed;

public sealed class IncreasedProcessed : IIncreaseProcessed
{
    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IncreaseProcessed increase)
    {
        IParser parser = increase.TakeOwner();
        LinkFromParserBag link = parser
            .OwnedLinks()
            .FindConcrete(l => new CompareLinkIdentityById(l, increase.TakeIncreaserId()));
        return link.Any()
            ? parser.IncreaseProcessed(link.Link())
            : new ParserLinkIsNotFoundInParserError(parser);
    }
}
