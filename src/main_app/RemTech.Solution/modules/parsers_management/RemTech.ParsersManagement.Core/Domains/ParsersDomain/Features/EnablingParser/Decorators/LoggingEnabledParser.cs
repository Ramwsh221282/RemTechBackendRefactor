using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Decorators;

public sealed class LoggingEnabledParser(ICustomLogger logger, IEnabledParser inner)
    : IEnabledParser
{
    public Status<IParser> Enable(EnableParser enable)
    {
        logger.Info("Включение парсера.");
        Status<IParser> enabled = inner.Enable(enable);
        if (enabled.IsSuccess)
        {
            logger.Info(new ParserPrint(enabled.Value).Read());
            logger.Info("Включение парсера завершено");
            return enabled;
        }
        logger.Error("Ошибка: {0}.", enabled.Error.ErrorText);
        return enabled;
    }
}
