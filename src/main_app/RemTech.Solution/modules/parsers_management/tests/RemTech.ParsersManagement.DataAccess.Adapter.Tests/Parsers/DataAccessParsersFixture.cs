using Npgsql;
using RemTech.Logging.Adapter;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using Serilog;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers;

public sealed class DataAccessParsersFixture : IDisposable
{
    private readonly ParsersDatabaseBakery _up;
    private readonly DatabaseConfiguration _configuration;
    private readonly ILogger _logger;

    public DataAccessParsersFixture()
    {
        _configuration = new DatabaseConfiguration("appsettings.json");
        _up = new ParsersDatabaseBakery(_configuration);
        _up.Up();
        _logger = new LoggerSource().Logger();
    }

    public IParsers Parsers()
    {
        NpgsqlDataSource source = new NpgsqlDataSourceBuilder(
            _configuration.ConnectionString
        ).Build();
        PgTransactionalParsers transactional = new(source, new PgParsers(source));
        return transactional;
    }

    public ILogger Logger() => _logger;

    public void Dispose()
    {
        _up.Down().Wait();
        _up.Up();
    }
}
