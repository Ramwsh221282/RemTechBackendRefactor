using Mailing.Domain.General;

namespace Mailing.Domain.Postmans.Factories.Statistics;

public sealed class PostmanLoggingStatisticsFactory(Serilog.ILogger logger, IPostmanStatisticsFactory factory)
    : PostmanStatisticsFactoryEnvelope(factory)
{
    public override IPostmanStatistics Construct()
    {
        logger.Information("Создание объекта Postman Statistics.");
        IPostmanStatistics statistics = base.Construct();
        logger.Information("Postman Statistics был создан.");
        return statistics;
    }

    public override IPostmanStatistics Construct(int sendLimit, int currentSend)
    {
        logger.Information("Создание объекта Postman Statistics.");
        logger.Information("Входные параметры: sendLimit: {sendLimit} currentSend: {currentSend}", sendLimit,
            currentSend);
        try
        {
            IPostmanStatistics statistics = base.Construct(sendLimit, currentSend);
            logger.Information("Postman Statistics был создан.");
            return statistics;
        }
        catch (InvalidObjectStateException ex)
        {
            logger.Error("Не удается создать объект Postman Statistics: {Message}.", ex.Message);
            throw;
        }
    }
}