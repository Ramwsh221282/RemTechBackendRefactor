using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Decorators;

public sealed class LoggingRemovedParserLink(ILogger logger, IRemovedParserLink inner)
    : IRemovedParserLink
{
    public Status<IParserLink> Removed(RemoveParserLink remove)
    {
        Status<IParserLink> link = inner.Removed(remove);
        if (link.IsSuccess)
        {
            logger.Information("Удалена ссылка у парсера.");
            return link;
        }
        logger.Error("Ошибка: {0}.", link.Error.ErrorText);
        return link;
    }
}
