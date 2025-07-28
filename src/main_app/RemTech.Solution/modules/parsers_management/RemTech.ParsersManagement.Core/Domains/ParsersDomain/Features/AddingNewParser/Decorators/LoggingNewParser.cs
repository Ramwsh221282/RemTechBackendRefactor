using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Decorators;

public sealed class LoggingNewParser(ILogger logger, INewParser inner) : INewParser
{
    public Status<IParser> Register(AddNewParser add)
    {
        Status<IParser> parser = inner.Register(add);
        if (parser.IsSuccess)
        {
            logger.Information("Парсер добавлен. {print}", new ParserPrint(parser.Value).Read());
            return parser;
        }
        logger.Error("Ошибка: {Error}.", parser.Error.ErrorText);
        return parser;
    }
}
