using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Decorators;

public sealed class LoggingDisabledParser(ILogger logger, IDisabledParser inner)
    : IDisabledParser
{
    public Status<IParser> Disable(DisableParser disable)
    {
        logger.Information("Отключение парсера.");
        Status<IParser> disabled = inner.Disable(disable);
        if (disabled.IsSuccess)
        {
            logger.Information("Парсер отключен. {print}.", new ParserPrint(disabled.Value).Read());
            return disabled;
        }
        logger.Error("Ошибка: {0}.", disabled.Error.ErrorText);
        return disabled;
    }
}
