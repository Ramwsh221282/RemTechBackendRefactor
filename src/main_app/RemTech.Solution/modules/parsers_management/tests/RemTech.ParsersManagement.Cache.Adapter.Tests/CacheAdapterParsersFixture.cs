using RemTech.Logging.Library;
using RemTech.ParserManagement.Cache.Adapter;
using RemTech.ParserManagement.Cache.Adapter.Configuration;
using RemTech.ParserManagement.Cache.Adapter.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests;

public sealed class CacheAdapterParsersFixture : IDisposable
{
    private readonly RedisParsers _cached;
    private readonly ICustomLogger _logger;
    private readonly ParsersSource _parsers;

    public CacheAdapterParsersFixture()
    {
        RedisConfiguration configuration = new("appsettings.json");
        RedisCacheEngine engine = new(configuration);
        _cached = new RedisParsers(engine);
        _logger = new MokLogger();
        MokParsers parsers = new();
        MokValidParsers valid = new(parsers);
        MokTransactionalParsers transactional = new(valid);
        _parsers = new ParsersSource(valid, transactional);
    }

    public void Dispose()
    {
        _cached.DropArray().Wait();
    }

    public ParsersSource Parsers() => _parsers;

    public IParsersCache CachedParsers() =>
        new LoggingParsersCache(_logger, new ParsersCache(_cached));

    public ICustomLogger Logger() => _logger;
}
