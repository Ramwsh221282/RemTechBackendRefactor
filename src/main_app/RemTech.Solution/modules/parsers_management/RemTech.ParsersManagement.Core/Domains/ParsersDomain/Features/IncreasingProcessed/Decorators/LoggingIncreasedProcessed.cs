using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Decorators;

public sealed class LoggingIncreasedProcessed(ICustomLogger logger, IIncreaseProcessed inner)
    : IIncreaseProcessed
{
    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IncreaseProcessed increase)
    {
        IParser parser = increase.TakeOwner();
        logger.Info(
            "Увеличение количества обработанных объявлений у парсера ID: {0}, название: {1}, тип: {2}, домен: {3}",
            parser.Identification().ReadId().GuidValue(),
            parser.Identification().ReadName().NameString().StringValue(),
            parser.Identification().ReadType().Read().StringValue(),
            parser.Domain().Read().NameString().StringValue()
        );
        logger.Info(
            "Предыдущее число обработанных: {0}.",
            parser.WorkedStatistics().ProcessedAmount().Read().Read()
        );
        Status<ParserStatisticsIncreasement> increasement = inner.IncreaseProcessed(increase);
        if (increasement.IsSuccess)
        {
            logger.Info("Новое число обработанных: {0}.", increasement.Value.CurrentProcessed());
            logger.Info("Статистика увеличена.");
            return increasement;
        }
        logger.Error("Ошибка: {0}.", increasement.Error.ErrorText);
        return increasement;
    }
}
