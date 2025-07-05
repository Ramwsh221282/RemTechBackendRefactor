using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Decorators;

public sealed class LoggingNewParser(ICustomLogger logger, INewParser inner) : INewParser
{
    public Status<IParser> Register(AddNewParser add)
    {
        logger.Info("Добавление нового парсера.");
        Status<IParser> parser = inner.Register(add);
        if (parser.IsSuccess)
        {
            logger.Info(new ParserPrint(parser.Value).Read());
            logger.Info("Парсер добавлен.");
            return parser;
        }
        logger.Error("Ошибка: {0}.", parser.Error.ErrorText);
        return parser;
    }
}
