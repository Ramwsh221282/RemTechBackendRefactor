using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

public interface IParser
{
    ParserIdentity Identification();

    ParserStatistic WorkedStatistics();

    ParserSchedule WorkSchedule();

    ParserState WorkState();

    ParserLinksBag OwnedLinks();

    ParserServiceDomain Domain();

    Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link);

    Status ChangeState(NotEmptyString stateString);

    Status Enable();

    Status Disable();

    Status ChangeWaitDays(PositiveInteger waitDays);

    Status<IParserLink> Put(IParserLink link);

    Status<IParserLink> Drop(IParserLink link);

    Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity);

    Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed);

    Status Stop();

    Status Start();
}
