using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Decorators;

public sealed class LoggingDisabledParser(ICustomLogger logger, IDisabledParser inner)
    : IDisabledParser
{
    public Status<IParser> Disable(DisableParser disable)
    {
        logger.Info("Отключение парсера.");
        Status<IParser> disabled = inner.Disable(disable);
        if (disabled.IsSuccess)
        {
            logger.Info(new ParserPrint(disabled.Value).Read());
            logger.Info("Парсер отключен.");
            return disabled;
        }
        logger.Error("Ошибка: {0}.", disabled.Error.ErrorText);
        return disabled;
    }
}
