using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Decorators;

public sealed class LoggingNewParserLink(ILogger logger, INewParserLink inner)
    : INewParserLink
{
    public Status<IParserLink> Register(AddParserLink addLink)
    {
        IParser parser = addLink.TakeOwner();
        ParserIdentity identification = parser.Identification();
        logger.Information(
            "Добавление ссылки парсеру ID: {id}, название: {name}.",
            (Guid)identification.ReadId(),
            (string)identification.ReadName().NameString()
        );
        Status<IParserLink> addedLink = inner.Register(addLink);
        if (addedLink.IsSuccess)
        {
            logger.Information("Добавлена ссылка парсеру. {print}", new ParserLinkPrint(addedLink.Value).Read());
            return addedLink;
        }
        logger.Error("Ошибка: {0}.", addedLink.Error.ErrorText);
        return addedLink;
    }
}
