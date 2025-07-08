using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;

public sealed class TransactionalLoggingParser(ICustomLogger logger, ITransactionalParser origin)
    : ITransactionalParser
{
    private const string Context = "ТРАНЗАКЦИОННЫЙ ПАРСЕР";

    public ParserIdentity Identification()
    {
        return origin.Identification();
    }

    public ParserStatistic WorkedStatistics()
    {
        return origin.WorkedStatistics();
    }

    public ParserSchedule WorkSchedule()
    {
        return origin.WorkSchedule();
    }

    public ParserState WorkState()
    {
        return origin.WorkState();
    }

    public ParserLinksBag OwnedLinks()
    {
        return origin.OwnedLinks();
    }

    public ParserServiceDomain Domain()
    {
        return origin.Domain();
    }

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link)
    {
        Status<ParserStatisticsIncreasement> increasement = origin.IncreaseProcessed(link);
        if (increasement.IsSuccess)
            logger.Info("{0}. Добавлено увеличение количества обработанных объявлений.", Context);
        return increasement;
    }

    public Status ChangeState(NotEmptyString stateString)
    {
        Status status = origin.ChangeState(stateString);
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено смена состояния парсера.", Context);
        return status;
    }

    public Status Enable()
    {
        Status status = origin.Enable();
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено включение парсера", Context);
        return status;
    }

    public Status Disable()
    {
        Status status = origin.Disable();
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено выключение парсера.", Context);
        return status;
    }

    public Status ChangeWaitDays(PositiveInteger waitDays)
    {
        Status status = origin.ChangeWaitDays(waitDays);
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено изменение дней ожидания.", Context);
        return status;
    }

    public Status<IParserLink> Put(IParserLink link)
    {
        Status<IParserLink> status = origin.Put(link);
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено добавление ссылки.", Context);
        return status;
    }

    public Status<IParserLink> Drop(IParserLink link)
    {
        Status<IParserLink> status = origin.Drop(link);
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено удаление ссылки.", Context);
        return status;
    }

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity)
    {
        Status<IParserLink> status = origin.ChangeActivityOf(link, nextActivity);
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено изменение активности ссылки.", Context);
        return status;
    }

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed)
    {
        Status<IParserLink> status = origin.Finish(link, elapsed);
        if (status.IsSuccess)
            logger.Info("{0}. Добавлено финиширование ссылки.", Context);
        return status;
    }

    public Status Stop()
    {
        Status status = origin.Stop();
        if (status.IsSuccess)
            logger.Info("{0}. Добавлена остановка парсера.", Context);
        return status;
    }

    public Status Start()
    {
        Status status = origin.Start();
        if (status.IsSuccess)
            logger.Info("{0}. Добавлен старт парсера.", Context);
        return status;
    }

    public void Dispose()
    {
        origin.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return origin.DisposeAsync();
    }

    public Task<Status> Save(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
