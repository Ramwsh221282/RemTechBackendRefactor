using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.DataSource.Adapter;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers;

public sealed class DataAccessParsersFixture : IDisposable
{
    private readonly ParsersManagementDbUp _up;
    private readonly ParsersManagementDatabaseConfiguration _configuration;
    private readonly ICustomLogger _logger;

    public DataAccessParsersFixture()
    {
        _configuration = new ParsersManagementDatabaseConfiguration("appsettings.json");
        _up = new ParsersManagementDbUp(_configuration);
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
