using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767073579)]
public sealed class CategorySeedingMigration : Migration
{
	public override void Up()
	{
		Execute.EmbeddedScript(
			"Vehicles.Infrastructure.Categories.SeedingImplementation.Scripts.0002.categories_seeding.sql"
		);
	}

	public override void Down() { }
}
