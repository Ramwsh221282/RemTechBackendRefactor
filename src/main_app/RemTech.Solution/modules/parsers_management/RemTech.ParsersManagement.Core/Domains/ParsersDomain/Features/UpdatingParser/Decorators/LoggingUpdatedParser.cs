using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;

public sealed class LoggingUpdatedParser(ILogger logger, IUpdatedParser inner)
    : IUpdatedParser
{
    public Status<IParser> Updated(UpdateParser update)
    {
        Status<IParser> updated = inner.Updated(update);
        if (updated.IsSuccess)
        {
            logger.Information("Состояние и(или) дни ожидания парсера обновлены.");
            return updated;
        }
        logger.Error("Ошибка: {0}.", updated.Error.ErrorText);
        return updated;
    }
}
