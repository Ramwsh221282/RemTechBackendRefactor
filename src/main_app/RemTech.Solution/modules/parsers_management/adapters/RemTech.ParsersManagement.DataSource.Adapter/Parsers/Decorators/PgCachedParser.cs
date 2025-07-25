using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class PgCachedParser(IParsersCache cache, IParser origin) : IParser
{
    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();

    public ParserIdentity Identification() => origin.Identification();

    public ParserStatistic WorkedStatistics() => origin.WorkedStatistics();

    public ParserSchedule WorkSchedule() => origin.WorkSchedule();

    public ParserState WorkState() => origin.WorkState();

    public ParserLinksBag OwnedLinks() => origin.OwnedLinks();

    public ParserServiceDomain Domain() => origin.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link)
    {
        Status<ParserStatisticsIncreasement> status = origin.IncreaseProcessed(link);
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status ChangeState(NotEmptyString stateString)
    {
        Status changing = origin.ChangeState(stateString);
        if (changing.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return changing;
    }

    public Status Enable()
    {
        Status status = origin.Enable();
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status Disable()
    {
        Status status = origin.Disable();
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status ChangeWaitDays(PositiveInteger waitDays)
    {
        Status status = origin.ChangeWaitDays(waitDays);
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status<IParserLink> Put(IParserLink link)
    {
        Status<IParserLink> status = origin.Put(link);
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status<IParserLink> Drop(IParserLink link)
    {
        Status<IParserLink> status = origin.Drop(link);
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity)
    {
        Status<IParserLink> status = origin.ChangeActivityOf(link, nextActivity);
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed)
    {
        Status<IParserLink> status = origin.Finish(link, elapsed);
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status Stop()
    {
        Status status = origin.Stop();
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }

    public Status Start()
    {
        Status status = origin.Start();
        if (status.IsSuccess)
            cache.Invalidate(new ParserCacheJson(origin));
        return status;
    }
}
