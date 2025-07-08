using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class TransactionalCachingParser(IParsersCache cache, ITransactionalParser origin)
    : ITransactionalParser
{
    public ParserIdentity Identification() => origin.Identification();

    public ParserStatistic WorkedStatistics() => origin.WorkedStatistics();

    public ParserSchedule WorkSchedule() => origin.WorkSchedule();

    public ParserState WorkState() => origin.WorkState();

    public ParserLinksBag OwnedLinks() => origin.OwnedLinks();

    public ParserServiceDomain Domain() => origin.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link) =>
        origin.IncreaseProcessed(link);

    public Status ChangeState(NotEmptyString stateString) => origin.ChangeState(stateString);

    public Status Enable() => origin.Enable();

    public Status Disable() => origin.Disable();

    public Status ChangeWaitDays(PositiveInteger waitDays) => origin.ChangeWaitDays(waitDays);

    public Status<IParserLink> Put(IParserLink link) => origin.Put(link);

    public Status<IParserLink> Drop(IParserLink link) => origin.Drop(link);

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity) =>
        origin.ChangeActivityOf(link, nextActivity);

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed) =>
        origin.Finish(link, elapsed);

    public Status Stop() => origin.Stop();

    public Status Start() => origin.Start();

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();

    public async Task<Status> Save(CancellationToken ct = default)
    {
        Status saving = await origin.Save(ct);
        if (saving.IsSuccess)
            await cache.Invalidate(new ParserCacheJson(origin));
        return saving;
    }
}
