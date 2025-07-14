using Npgsql;
using RemTech.ParsedAdvertisements.Core.Tests.Moks;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Characteristics.Decorators;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.ParsedAdvertisements.DataSource.Tests;

public sealed class DataSourceTestsFixture : IDisposable
{
    private readonly DatabaseConfiguration _configuration;

    public DataSourceTestsFixture()
    {
        _configuration = new DatabaseConfiguration("appsettings.json");
        DatabaseBakery bakery = new(_configuration);
        bakery.Up(typeof(PgVehicleKinds).Assembly);
    }

    public void Dispose()
    {
        DatabaseBakery bakery = new(_configuration);
        bakery
            .Down(
                "parsed_advertisements_module.cities",
                "parsed_advertisements_module.parsed_vehicle_characteristics",
                "shared_advertisements_module.contained_items",
                "parsed_advertisements_module.parsed_vehicles",
                "parsed_advertisements_module.vehicle_kinds",
                "parsed_advertisements_module.vehicle_brands",
                "parsed_advertisements_module.geos",
                "parsed_advertisements_module.vehicle_characteristics",
                "parsers_management_module.parser_links",
                "parsers_management_module.parsers"
            )
            .Wait();
    }

    public IAsyncVehicleKinds VehicleKinds()
    {
        NpgsqlDataSource source = Engine();
        return new ValidatingTextSearchVehicleKinds(
            new TsQueryPgVehicleKinds(
                source,
                new PgTgrmPgVehicleKinds(
                    source,
                    new ValidatingPgVehicleKinds(new PgVehicleKinds(source))
                )
            )
        );
    }

    public IAsyncVehicleBrands VehicleBrands()
    {
        NpgsqlDataSource source = Engine();
        return new LoggingPgVehicleBrands(
            new MokLogger(),
            new TsQueryVehicleBrands(
                source,
                new PgTgrmVehicleBrands(
                    source,
                    new ValidatingPgVehicleBrands(new PgVehicleBrands(source))
                )
            )
        );
    }

    public IAsyncGeoLocations Locations()
    {
        NpgsqlDataSource source = Engine();
        return new TsQueryPgGeoLocations(
            source,
            new TgrmPgGeoLocations(source, new ValidatingPgGeoLocations(new PgGeoLocations(source)))
        );
    }

    public IAsyncCharacteristics Characteristics()
    {
        NpgsqlDataSource source = Engine();
        return new TextSearchPgCharacteristics(
            source,
            new PgValidatingCharacteristics(new PgCharacteristics(source))
        );
    }

    public NpgsqlDataSource Engine()
    {
        return new NpgsqlDataSourceBuilder(_configuration.ConnectionString).Build();
    }
}
