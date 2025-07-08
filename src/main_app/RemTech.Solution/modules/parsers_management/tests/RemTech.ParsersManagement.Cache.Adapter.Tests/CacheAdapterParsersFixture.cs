using RemTech.Logging.Library;
using RemTech.ParserManagement.Cache.Adapter;
using RemTech.ParserManagement.Cache.Adapter.Configuration;
using RemTech.ParserManagement.Cache.Adapter.Parsers;
using RemTech.ParserManagement.Cache.Adapter.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.DataSource.Adapter;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests;

public sealed class CacheAdapterParsersFixture : IDisposable
{
    private readonly RedisConfiguration _redisConf;
    private readonly ParsersManagementDatabaseConfiguration _dbConf;
    private readonly MokLogger _logger;

    public CacheAdapterParsersFixture()
    {
        string settingsPath = "appsettings.json";
        _redisConf = new RedisConfiguration(settingsPath);
        _dbConf = new ParsersManagementDatabaseConfiguration(settingsPath);
        _logger = new MokLogger();
        ParsersManagementDbUp dbUp = new(_dbConf);
        dbUp.Up();
    }

    public ParserTestingToolkit Toolkit()
    {
        return new ParserTestingToolkit(_logger, Parsers());
    }

    public ParsersSource Parsers()
    {
        PostgreSqlEngine dbEngine = new(_dbConf);
        IParsersCache cached = CachedSource();
        return new ParsersSource(
            new PgLoggingParsers(
                _logger,
                new PgValidParsers(new PgCachingParsers(cached, new PgParsers(dbEngine)))
            ),
            new PgTransactionalLoggingParsers(
                _logger,
                new PgTransactionalCachingParsers(cached, new PgTransactionalParsers(dbEngine))
            )
        );
    }

    public IParsersCache CachedSource()
    {
        RedisCacheEngine engine = new(_redisConf);
        IParsersCache cache = new LoggingRedisParsers(_logger, new RedisParsers(engine));
        return cache;
    }

    public ICustomLogger Logger()
    {
        return _logger;
    }

    public void Dispose()
    {
        RedisParsers parsers = new(new RedisCacheEngine(_redisConf));
        parsers.DropArray().Wait();
        ParsersManagementDbUp dbUp = new(_dbConf);
        dbUp.Down().Wait();
    }
}
