using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Decorators;

public sealed class LoggingEnabledParser(ILogger logger, IEnabledParser inner)
    : IEnabledParser
{
    public Status<IParser> Enable(EnableParser enable)
    {
        logger.Information("Включение парсера.");
        Status<IParser> enabled = inner.Enable(enable);
        if (enabled.IsSuccess)
        {
            logger.Information("Включение парсера завершено. {Print}", new ParserPrint(enabled.Value).Read());
            return enabled;
        }
        logger.Error("Ошибка: {0}.", enabled.Error.ErrorText);
        return enabled;
    }
}
