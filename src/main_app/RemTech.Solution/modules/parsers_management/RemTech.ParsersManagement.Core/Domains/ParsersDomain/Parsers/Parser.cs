using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;

public sealed class Parser : IParser
{
    private readonly ParserIdentity _identity;
    private readonly ParserStatistic _statistics;
    private ParserLinksBag _links;
    private ParserSchedule _schedule;
    private ParserState _state;

    public Parser(Name name, ParsingType type, ParserServiceDomain domain)
    {
        _identity = new ParserIdentity(NotEmptyGuid.New(), name, type, domain);
        _statistics = new ParserStatistic();
        _schedule = new ParserSchedule();
        _state = ParserState.Disabled();
        _links = new ParserLinksBag();
    }

    public Parser(
        ParserIdentity identity,
        ParserStatistic statistic,
        ParserSchedule schedule,
        ParserState state,
        ParserLinksBag links
    )
    {
        _identity = identity;
        _statistics = statistic;
        _schedule = schedule;
        _state = state;
        _links = links;
    }

    public Parser(
        ParserIdentity identity,
        ParserStatistic statistic,
        ParserSchedule schedule,
        ParserState state
    )
    {
        _identity = identity;
        _statistics = statistic;
        _schedule = schedule;
        _state = state;
        _links = new ParserLinksBag();
    }

    public ParserIdentity Identification() => _identity;

    public ParserStatistic WorkedStatistics() => _statistics;

    public ParserSchedule WorkSchedule() => _schedule;

    public ParserState WorkState() => _state;

    public ParserLinksBag OwnedLinks() => _links;

    public ParserServiceDomain Domain() => _identity.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link)
    {
        _statistics.IncreaseProcessed();
        link.WorkedStatistic().IncreaseProcessed();
        return new ParserStatisticsIncreasement(this, link);
    }

    public Status<IParserLink> Put(IParserLink link)
    {
        _links += link;
        return link.Success();
    }

    public Status<IParserLink> Drop(IParserLink link)
    {
        _links -= link;
        return link.Success();
    }

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity)
    {
        Status changing = link.OtherActivity(nextActivity);
        return changing.IsFailure ? Status<IParserLink>.Failure(changing.Error) : link.Success();
    }

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed)
    {
        Status finishing = link.Finished(elapsed);
        return finishing.IsSuccess ? link.Success() : finishing.Error;
    }

    public Status Stop()
    {
        _state = ParserState.Waiting();
        _schedule = _schedule.Next();
        return Status.Success();
    }

    public Status Start()
    {
        _state = ParserState.Working();
        _statistics.Reset();
        return Status.Success();
    }

    public Status ChangeState(NotEmptyString stateString)
    {
        _state = ParserState.New(stateString);
        return Status.Success();
    }

    public Status Enable()
    {
        _state = ParserState.Waiting();
        return Status.Success();
    }

    public Status Disable()
    {
        _state = ParserState.Disabled();
        return Status.Success();
    }

    public Status ChangeWaitDays(PositiveInteger waitDays)
    {
        _schedule = _schedule.OtherWaitDays(waitDays);
        return Status.Success();
    }

    public void Dispose() { }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
