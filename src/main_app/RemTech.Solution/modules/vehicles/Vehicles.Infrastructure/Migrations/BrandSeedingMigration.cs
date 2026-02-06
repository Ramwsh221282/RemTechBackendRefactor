using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для начального заполнения таблицы брендов.
/// </summary>
[Migration(1767063770)]
public sealed class BrandSeedingMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, заполняя таблицу брендов начальными данными.
	/// </summary>
	public override void Up()
	{
		Execute.EmbeddedScript("Vehicles.Infrastructure.Brands.SeedingImplementation.Scripts.0002.brands_seeding.sql");
	}

	/// <summary>
	/// Откатывает миграцию. В данном случае откат не выполняется.
	/// </summary>
	public override void Down() { }
}
