using RemTech.ParsersManagement.DataSource.Adapter;
using RemTech.ParsersManagement.DataSource.Adapter.DataAccessConfiguration;
using RemTech.ParsersManagement.DataSource.Adapter.Parsers;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.DataAccess.Adapter.Tests.Parsers;

public sealed class DataAccessParsersFixture : IDisposable
{
    private readonly ParsersManagementDbUp _up;
    private readonly ParsersManagementDatabaseConfiguration _configuration;
    private readonly PgParsers _parsers;
    private readonly MokLogger _logger;

    public DataAccessParsersFixture()
    {
        _configuration = new ParsersManagementDatabaseConfiguration("appsettings.json");
        _up = new ParsersManagementDbUp(_configuration);
        _up.Up();
        _parsers = new PgParsers(new PostgreSqlEngine(_configuration));
        _logger = new MokLogger();
    }

    public PgParsers AccessPgParsers()
    {
        return _parsers;
    }

    public MokLogger AccessMokLogger()
    {
        return _logger;
    }

    public MokTransactionalParsers AccessMokTransactionalParsers() =>
        new(new MokValidParsers(new MokParsers()));

    public void Dispose()
    {
        _up.Down().Wait();
        _up.Up();
    }
}
