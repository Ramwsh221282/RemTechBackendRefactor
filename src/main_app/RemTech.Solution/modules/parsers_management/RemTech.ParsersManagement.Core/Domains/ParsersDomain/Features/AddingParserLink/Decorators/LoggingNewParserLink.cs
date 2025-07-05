using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Decorators;

public sealed class LoggingNewParserLink(ICustomLogger logger, INewParserLink inner)
    : INewParserLink
{
    public Status<IParserLink> Register(AddParserLink addLink)
    {
        IParser parser = addLink.TakeOwner();
        ParserIdentity identification = parser.Identification();
        logger.Info(
            "Добавление ссылки парсеру ID: {0}, название: {1}.",
            identification.ReadId().GuidValue(),
            identification.ReadName().NameString()
        );
        Status<IParserLink> addedLink = inner.Register(addLink);
        if (addedLink.IsSuccess)
        {
            logger.Info(new ParserLinkPrint(addedLink.Value).Read());
            logger.Info("Добавлена ссылка парсеру.");
            return addedLink;
        }
        logger.Error("Ошибка: {0}.", addedLink.Error.ErrorText);
        return addedLink;
    }
}
