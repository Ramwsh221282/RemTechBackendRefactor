using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports.Database;
using RemTech.ParsersManagement.DataSource.Adapter;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers;

public sealed class DataAccessParsersFixture : IDisposable
{
    private readonly ParsersDatabaseBakery _up;
    private readonly DatabaseConfiguration _configuration;
    private readonly ICustomLogger _logger;

    public DataAccessParsersFixture()
    {
        _configuration = new DatabaseConfiguration("appsettings.json");
        _up = new ParsersDatabaseBakery(_configuration);
        _up.Up();
        _logger = new MokLogger();
    }

    public ParsersSource ParsersSource()
    {
        PostgreSqlEngine engine = new(_configuration);
        PgParsers parsers = new(engine);
        PgTransactionalParsers transactional = new(engine);
        return new ParsersSource(parsers, transactional);
    }

    public ICustomLogger Logger() => _logger;

    public void Dispose()
    {
        _up.Down().Wait();
        _up.Up();
    }
}
