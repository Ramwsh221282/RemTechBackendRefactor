using RemTech.Core.Shared.Primitives;
using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers.Decorators;

public sealed class LoggingRedisParsers(ICustomLogger logger, IParsersCache cache) : IParsersCache
{
    private const string Context = "REDIS PARSERS";

    public async Task Invalidate(ParserCacheJson json)
    {
        logger.Info("{0}. {1}. Начата.", Context, "Инвалидция.");
        await cache.Invalidate(json);
        logger.Info("{0}. {1}. Закончена.", Context, "Инвалидция.");
    }

    public async Task<MaybeBag<IParser>> Get(ParserCacheKey key)
    {
        logger.Info("{0}. Получение парсера из кеша по ID: {1}.", Context, (string)key);
        MaybeBag<IParser> parser = await cache.Get(key);
        if (parser.Any())
            logger.Info("{0}. Парсер найден.", Context);
        else
            logger.Warn("{0}. Парсер не найден.", Context);
        return parser;
    }

    public async Task<MaybeBag<IParser>> Get(Name name)
    {
        logger.Info("{0}. Получение парсера из кеша по имени: {1}.", Context, (string)name);
        MaybeBag<IParser> parser = await cache.Get(name);
        if (parser.Any())
            logger.Info("{0}. Парсер найден.", Context);
        else
            logger.Warn("{0}. Парсер не найден.", Context);
        return parser;
    }

    public async Task<MaybeBag<IParser>> Get(ParsingType type, NotEmptyString domain)
    {
        logger.Info(
            "{0}. Получение парсера из кеша по типу: {1} и домену: {2}.",
            Context,
            (string)type.Read(),
            domain.StringValue()
        );
        MaybeBag<IParser> parser = await cache.Get(type, domain);
        if (parser.Any())
            logger.Info("{0}. Парсер найден.", Context);
        else
            logger.Warn("{0}. Парсер не найден.", Context);
        return parser;
    }

    public async Task<IParser[]> Get()
    {
        logger.Info("{0}. Получение массива парсеров из кеша.", Context);
        IParser[] parsers = await cache.Get();
        logger.Info("{0}. Получено {1} парсеров из кеша.", Context, parsers.Length);
        return parsers;
    }
}
