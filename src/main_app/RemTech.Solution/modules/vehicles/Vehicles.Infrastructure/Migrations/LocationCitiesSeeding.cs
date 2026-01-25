using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767086400)]
public sealed class LocationCitiesSeeding : Migration
{
    public override void Up() => Execute.EmbeddedScript("Vehicles.Infrastructure.Locations.SeedingImplementation.cities_seeding.sql");

    public override void Down() { }
}
