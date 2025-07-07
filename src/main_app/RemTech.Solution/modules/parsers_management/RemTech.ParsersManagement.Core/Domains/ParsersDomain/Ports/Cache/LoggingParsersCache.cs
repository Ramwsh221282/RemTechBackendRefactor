using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;

public sealed class LoggingParsersCache(ICustomLogger logger, IParsersCache cache) : IParsersCache
{
    public async Task Invalidate(ParserCacheJson json)
    {
        logger.Info("Инвалидация кеша парсеров.");
        await cache.Invalidate(json);
        logger.Info("Инвалидация кеша парсеров прошла успешна.");
    }

    public async Task<MaybeBag<IParser>> Get(ParserCacheKey key)
    {
        logger.Info("Получение парсера из кеша:");
        MaybeBag<IParser> maybeParser = await cache.Get(key);
        if (maybeParser.Any())
            logger.Info(new ParserPrint(maybeParser.Take()).Read());
        else
            logger.Warn("В кеше не найден парсер с ключом: {0}", (string)key);
        return maybeParser;
    }

    public async Task<IParser[]> Get()
    {
        logger.Info("Получение массива парсеров из кеша.");
        IParser[] parsers = await cache.Get();
        if (parsers.Length == 0)
        {
            logger.Warn("В кеше не было парсеров.");
            return parsers;
        }
        foreach (IParser parser in parsers)
            logger.Info(new ParserPrint(parser).Read());
        return parsers;
    }
}
