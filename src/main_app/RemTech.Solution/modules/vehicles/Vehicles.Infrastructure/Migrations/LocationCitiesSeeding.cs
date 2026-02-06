using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для начального заполнения городов локаций.
/// </summary>
[Migration(1767086400)]
public sealed class LocationCitiesSeeding : Migration
{
	/// <summary>
	/// Выполняет миграцию, заполняя таблицу городов начальными данными.
	/// </summary>
	public override void Up()
	{
		Execute.EmbeddedScript("Vehicles.Infrastructure.Locations.SeedingImplementation.cities_seeding.sql");
	}

	/// <summary>
	/// Откатывает миграцию. В данном случае откат не выполняется.
	/// </summary>
	public override void Down() { }
}
