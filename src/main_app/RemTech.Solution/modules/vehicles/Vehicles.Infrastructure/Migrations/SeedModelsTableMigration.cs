using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767096761)]
public sealed class SeedModelsTableMigration : Migration
{
	public override void Up()
	{
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0002.models_seeding_1.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0003.models_seeding_2.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0004.models_seeding_3.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0005.models_seeding_4.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0006.models_seeding_5.sql");
	}

	public override void Down() { }
}
