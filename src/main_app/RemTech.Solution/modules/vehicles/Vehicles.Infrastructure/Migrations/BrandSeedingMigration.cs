using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767063770)]
public sealed class BrandSeedingMigration : Migration
{
    public override void Up() => Execute.EmbeddedScript("Vehicles.Infrastructure.Brands.SeedingImplementation.Scripts.0002.brands_seeding.sql");

    public override void Down() { }
}
