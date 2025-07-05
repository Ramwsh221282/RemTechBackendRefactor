using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Decorators;

public sealed class LoggingChangedLinkActivity(ICustomLogger logger, IChangedLinkActivity inner)
    : IChangedLinkActivity
{
    public Status<IParserLink> Changed(ChangeLinkActivity change)
    {
        IParser parser = change.TakeOwner();
        logger.Info(
            "Изменение активности ссылки у парсера ID: {0}, название: {1}, тип: {2}, домен: {3}.",
            parser.Identification().ReadId().GuidValue(),
            parser.Identification().ReadName().NameString().StringValue(),
            parser.Identification().ReadType().Read().StringValue(),
            parser.Domain().Read().NameString().StringValue()
        );
        Status<IParserLink> changed = inner.Changed(change);
        if (changed.IsSuccess)
        {
            logger.Info("Активность ссылки изменилась на: {0}.", changed.Value.Activity().Read());
            return changed;
        }
        logger.Error("Ошибка: {0}.", changed.Error.ErrorText);
        return changed;
    }
}
