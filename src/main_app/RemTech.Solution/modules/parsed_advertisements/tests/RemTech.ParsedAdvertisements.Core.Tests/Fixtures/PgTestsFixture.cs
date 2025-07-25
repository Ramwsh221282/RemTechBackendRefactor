using RemTech.Logging.Adapter;
using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.ParsedAdvertisements.Core.Tests.Fixtures;

public sealed class PgTestsFixture : IDisposable
{
    private readonly DatabaseConfiguration _configuration;
    private readonly ICustomLogger _logger;

    public PgTestsFixture()
    {
        _configuration = new DatabaseConfiguration("appsettings.json");
        _logger = new ConsoleLogger();
        DatabaseBakery bakery = new(_configuration);
        bakery.Up(typeof(Vehicle).Assembly);
    }

    public DatabaseConfiguration DbConfig() => _configuration;
    public ICustomLogger Logger() => _logger;

    public void Dispose()
    {
        DatabaseBakery bakery = new(_configuration);
        bakery
            .Down(
                "parsed_advertisements_module.vehicle_brand_models",
                "parsed_advertisements_module.vehicle_models",
                "parsed_advertisements_module.cities",
                "parsed_advertisements_module.parsed_vehicle_characteristics",
                "parsed_advertisements_module.parsed_vehicles",
                "parsed_advertisements_module.vehicle_kinds",
                "parsed_advertisements_module.vehicle_brands",
                "parsed_advertisements_module.geos",
                "parsed_advertisements_module.vehicle_characteristics"
            )
            .Wait();
    }
}
