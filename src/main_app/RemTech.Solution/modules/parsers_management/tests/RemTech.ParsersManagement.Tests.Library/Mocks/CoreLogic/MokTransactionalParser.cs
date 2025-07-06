using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

public sealed class MokTransactionalParser(IParser inner, MokValidParsers source)
    : ITransactionalParser
{
    public ParserIdentity Identification() => inner.Identification();

    public ParserStatistic WorkedStatistics() => inner.WorkedStatistics();

    public ParserSchedule WorkSchedule() => inner.WorkSchedule();

    public ParserState WorkState() => inner.WorkState();

    public ParserLinksBag OwnedLinks() => inner.OwnedLinks();

    public ParserServiceDomain Domain() => inner.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link) =>
        inner.IncreaseProcessed(link);

    public Status ChangeState(NotEmptyString stateString) => inner.ChangeState(stateString);

    public Status Enable() => inner.Enable();

    public Status Disable() => inner.Disable();

    public Status ChangeWaitDays(PositiveInteger waitDays) => inner.ChangeWaitDays(waitDays);

    public Status<IParserLink> Put(IParserLink link) => inner.Put(link);

    public Status<IParserLink> Drop(IParserLink link) => inner.Drop(link);

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity) =>
        inner.ChangeActivityOf(link, nextActivity);

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed) =>
        inner.Finish(link, elapsed);

    public Status Stop() => inner.Stop();

    public Status Start() => inner.Start();

    public void Dispose() { }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task<Status> Save(CancellationToken ct = default)
    {
        if (!source.ContainsParser(inner))
            return Task.FromResult(Status.Failure(Error.NotFound("Парсер не существует.")));
        source.SaveParser(inner);
        return Task.FromResult(Status.Success());
    }
}
