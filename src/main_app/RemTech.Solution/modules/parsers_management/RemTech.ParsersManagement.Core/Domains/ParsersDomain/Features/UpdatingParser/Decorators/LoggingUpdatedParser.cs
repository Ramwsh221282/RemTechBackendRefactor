using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class LoggingUpdatedParser(ICustomLogger logger, IUpdatedParser inner)
    : IUpdatedParser
{
    public Status<IParser> Updated(UpdateParser update)
    {
        logger.Info("Обновление состояния и(или) дней ожидания парсера.");
        Status<IParser> updated = inner.Updated(update);
        if (updated.IsSuccess)
        {
            logger.Info(new ParserPrint(updated.Value).Read());
            logger.Info("Состояние и(или) дни ожидания парсера обновлены.");
            return updated;
        }
        logger.Error("Ошибка: {0}.", updated.Error.ErrorText);
        return updated;
    }
}
