using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Decorators;

public sealed class LoggingFinishedParserLink(ICustomLogger logger, IFinishedParserLink inner)
    : IFinishedParserLink
{
    public Status<IParserLink> Finished(FinishParserLink finish)
    {
        IParser parser = finish.TakeOwner();
        logger.Info(
            "Завершение работы ссылки парсера ID: {0}, название: {1}, тип: {2}, домен: {3}.",
            parser.Identification().ReadId().GuidValue(),
            parser.Identification().ReadName().NameString().StringValue(),
            parser.Identification().ReadType().Read().StringValue(),
            parser.Domain().Read().NameString().StringValue()
        );
        Status<IParserLink> finished = inner.Finished(finish);
        if (finished.IsSuccess)
        {
            IParserLink link = finished.Value;
            logger.Info(
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
