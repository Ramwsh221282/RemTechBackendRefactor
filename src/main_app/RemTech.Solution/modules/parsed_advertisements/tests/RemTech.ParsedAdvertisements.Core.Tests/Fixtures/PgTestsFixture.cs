using RemTech.Logging.Adapter;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using Serilog;

namespace RemTech.ParsedAdvertisements.Core.Tests.Fixtures;

public sealed class PgTestsFixture
{
    private readonly DatabaseConfiguration _configuration;
    private readonly ILogger _logger;

    public PgTestsFixture()
    {
        _configuration = new DatabaseConfiguration("appsettings.json");
        _logger = new LoggerSource().Logger();
        // DatabaseBakery bakery = new(_configuration);
        // bakery.Up(typeof(Vehicle).Assembly);
    }

    public DatabaseConfiguration DbConfig() => _configuration;
    public ILogger Logger() => _logger;
}
