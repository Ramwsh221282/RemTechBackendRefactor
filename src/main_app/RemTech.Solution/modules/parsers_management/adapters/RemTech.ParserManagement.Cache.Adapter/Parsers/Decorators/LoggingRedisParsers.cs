using RemTech.Core.Shared.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParserManagement.Cache.Adapter.Parsers.Decorators;

public sealed class LoggingRedisParsers(ILogger logger, IParsersCache cache) : IParsersCache
{
    private const string Context = "REDIS PARSERS";

    public async Task InvalidateAsync(ParserCacheJson json)
    {
        logger.Information("{0}. {1}. Начата.", Context, "Инвалидция.");
        await cache.InvalidateAsync(json);
        logger.Information("{0}. {1}. Закончена.", Context, "Инвалидция.");
    }

    public void Invalidate(ParserCacheJson json)
    {
        cache.Invalidate(json);
    }

    public async Task<MaybeBag<IParser>> Get(ParserCacheKey key)
    {
        logger.Information("{0}. Получение парсера из кеша по ID: {1}.", Context, (string)key);
        MaybeBag<IParser> parser = await cache.Get(key);
        if (parser.Any())
            logger.Information("{0}. Парсер найден.", Context);
        else
            logger.Warning("{0}. Парсер не найден.", Context);
        return parser;
    }

    public async Task<MaybeBag<IParser>> Get(Name name)
    {
        logger.Information("{0}. Получение парсера из кеша по имени: {1}.", Context, (string)name);
        MaybeBag<IParser> parser = await cache.Get(name);
        if (parser.Any())
            logger.Information("{0}. Парсер найден.", Context);
        else
            logger.Warning("{0}. Парсер не найден.", Context);
        return parser;
    }

    public async Task<MaybeBag<IParser>> Get(ParsingType type, NotEmptyString domain)
    {
        logger.Information(
            "{0}. Получение парсера из кеша по типу: {1} и домену: {2}.",
            Context,
            (string)type.Read(),
            domain.StringValue()
        );
        MaybeBag<IParser> parser = await cache.Get(type, domain);
        if (parser.Any())
            logger.Information("{0}. Парсер найден.", Context);
        else
            logger.Warning("{0}. Парсер не найден.", Context);
        return parser;
    }

    public async Task<IParser[]> Get()
    {
        logger.Information("{0}. Получение массива парсеров из кеша.", Context);
        IParser[] parsers = await cache.Get();
        logger.Information("{0}. Получено {1} парсеров из кеша.", Context, parsers.Length);
        return parsers;
    }
}
