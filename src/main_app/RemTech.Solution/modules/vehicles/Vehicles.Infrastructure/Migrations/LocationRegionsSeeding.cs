using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767086300)]
public sealed class LocationRegionsSeeding : Migration
{
	public override void Up()
	{
		Execute.EmbeddedScript("Vehicles.Infrastructure.Locations.SeedingImplementation.regions_seeding.sql");
	}

	public override void Down() { }
}
