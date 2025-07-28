using Npgsql;
using RemTech.Logging.Adapter;
using RemTech.ParserManagement.Cache.Adapter;
using RemTech.ParserManagement.Cache.Adapter.Configuration;
using RemTech.ParserManagement.Cache.Adapter.Parsers;
using RemTech.ParserManagement.Cache.Adapter.Parsers.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Cache;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers.Decorators;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using Serilog;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests;

public sealed class CacheAdapterParsersFixture : IDisposable
{
    private readonly RedisConfiguration _redisConf;
    private readonly DatabaseConfiguration _dbConf;
    private readonly ILogger _logger;

    public CacheAdapterParsersFixture()
    {
        string settingsPath = "appsettings.json";
        _redisConf = new RedisConfiguration(settingsPath);
        _dbConf = new DatabaseConfiguration(settingsPath);
        _logger = new LoggerSource().Logger();
        ParsersDatabaseBakery dbUp = new(_dbConf);
        dbUp.Up();
    }

    public ParserTestingToolkit Toolkit()
    {
        return new ParserTestingToolkit(_logger, Parsers());
    }

    public IParsers Parsers()
    {
        NpgsqlDataSource ds = new NpgsqlDataSourceBuilder(_dbConf.ConnectionString).Build();
        IParsersCache cached = CachedSource();
        return new PgLoggingParsers(
            _logger,
            new PgValidParsers(
                new PgCachingParsers(ds, cached, new PgTransactionalParsers(ds, new PgParsers(ds)))
            )
        );
    }

    public IParsersCache CachedSource()
    {
        RedisCacheEngine engine = new(_redisConf);
        IParsersCache cache = new LoggingRedisParsers(_logger, new RedisParsers(engine));
        return cache;
    }

    public ILogger Logger()
    {
        return _logger;
    }

    public void Dispose()
    {
        RedisParsers parsers = new(new RedisCacheEngine(_redisConf));
        parsers.DropArray().Wait();
        ParsersDatabaseBakery dbUp = new(_dbConf);
        dbUp.Down().Wait();
    }
}
