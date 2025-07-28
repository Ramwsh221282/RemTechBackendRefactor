using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Decorators;

public sealed class LoggingFinishedParserLink(ILogger logger, IFinishedParserLink inner)
    : IFinishedParserLink
{
    public Status<IParserLink> Finished(FinishParserLink finish)
    {
        Status<IParserLink> finished = inner.Finished(finish);
        if (finished.IsSuccess)
        {
            IParserLink link = finished.Value;
            logger.Information(
                "Работа ссылки завершена. Затрачено: {0} ч. {1} м. {2} с.",
                link.WorkedStatistic().WorkedTime().Hours().Read().Read(),
                link.WorkedStatistic().WorkedTime().Minutes().Read().Read(),
                link.WorkedStatistic().WorkedTime().Seconds().Read().Read()
            );
            return finished.Success();
        }
        logger.Error("Ошибка: {0}.", finished.Error.ErrorText);
        return finished;
    }
}
