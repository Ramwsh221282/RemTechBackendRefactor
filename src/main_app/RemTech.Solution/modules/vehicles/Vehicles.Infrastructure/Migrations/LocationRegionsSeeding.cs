using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для начального заполнения регионов локаций.
/// </summary>
[Migration(1767086300)]
public sealed class LocationRegionsSeeding : Migration
{
	/// <summary>
	/// Выполняет миграцию, заполняя таблицу регионов начальными данными.
	/// </summary>
	public override void Up()
	{
		Execute.EmbeddedScript("Vehicles.Infrastructure.Locations.SeedingImplementation.regions_seeding.sql");
	}

	/// <summary>
	/// Откатывает миграцию. В данном случае откат не выполняется.
	/// </summary>
	public override void Down() { }
}
