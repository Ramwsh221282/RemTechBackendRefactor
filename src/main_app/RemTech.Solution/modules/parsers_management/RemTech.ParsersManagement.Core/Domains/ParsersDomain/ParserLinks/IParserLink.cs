using RemTech.Core.Shared.Primitives;
using RemTech.Core.Shared.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;

public interface IParserLink : ISameBy
{
    ParserLinkStatistic WorkedStatistic();
    ParserLinkIdentity Identification();
    ParserLinkActivity Activity();
    ParserLinkUrl ReadUrl();
    Status OtherActivity(bool other);
    Status Finished(PositiveLong elapsed);
}
