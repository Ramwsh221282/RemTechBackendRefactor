using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.ParsedAdvertisements.DataSource.Tests;

public sealed class DataSourceTestsFixture : IDisposable
{
    private readonly DatabaseConfiguration _configuration;

    // public DataSourceTestsFixture()
    // {
    //     _configuration = new DatabaseConfiguration("appsettings.json");
    //     DatabaseBakery bakery = new(_configuration);
    //     bakery.Up(typeof(SqlSpeakingVehicleKinds).Assembly);
    // }

    public void Dispose()
    {
        // DatabaseBakery bakery = new(_configuration);
        // bakery
        //     .Down(
        //         "shared_advertisements_module.contained_items",
        //         "parsed_advertisements_module.vehicle_kinds",
        //         "parsed_advertisements_module.vehicle_brands",
        //         "parsed_advertisements_module.geos",
        //         "parsed_advertisements_module.vehicle_characteristics",
        //         "parsed_advertisements_module.parsed_vehicles",
        //         "parsed_advertisements_module.parsed_vehicle_characteristics"
        //     )
        //     .Wait();
    }

    // public PgDefaultConnectionSource Engine()
    // {
    //     return new PgDefaultConnectionSource(_configuration);
    // }
}
