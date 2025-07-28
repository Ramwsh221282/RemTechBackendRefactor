using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Decorators;

public sealed class LoggingIncreasedProcessed(ILogger logger, IIncreaseProcessed inner)
    : IIncreaseProcessed
{
    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IncreaseProcessed increase)
    {
        IParser parser = increase.TakeOwner();
        logger.Information(
            "Предыдущее число обработанных: {0}.",
            parser.WorkedStatistics().ProcessedAmount().Read().Read()
        );
        Status<ParserStatisticsIncreasement> increasement = inner.IncreaseProcessed(increase);
        if (increasement.IsSuccess)
        {
            logger.Information("Новое число обработанных: {0}.", increasement.Value.CurrentProcessed());
            return increasement;
        }
        logger.Error("Ошибка: {0}.", increasement.Error.ErrorText);
        return increasement;
    }
}
