using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Decorators;

public sealed class LoggingRemovedParserLink(ICustomLogger logger, IRemovedParserLink inner)
    : IRemovedParserLink
{
    public Status<IParserLink> Removed(RemoveParserLink remove)
    {
        IParser parser = remove.TakeOwner();
        ParserIdentity identification = parser.Identification();
        logger.Info(
            "Удаление ссылки у парсера с ID: {0}. Названием: {1}. Типом: {2}. Доменом: {3}.",
            identification.ReadId().GuidValue(),
            identification.ReadName().NameString().StringValue(),
            identification.ReadType().Read().StringValue(),
            parser.Domain().Read().NameString().StringValue()
        );
        Status<IParserLink> link = inner.Removed(remove);
        if (link.IsSuccess)
        {
            logger.Info(new ParserLinkPrint(link.Value).Read());
            logger.Info("Удалена ссылка у парсера.");
            return link;
        }
        logger.Error("Ошибка: {0}.", link.Error.ErrorText);
        return link;
    }
}
