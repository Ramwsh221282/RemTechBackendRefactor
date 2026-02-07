using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы регионов.
/// </summary>
[Migration(1767086100)]
public sealed class LocationRegionsMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу регионов.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.regions (
			    id UUID PRIMARY KEY,
			    name VARCHAR(256) NOT NULL,
			    kind VARCHAR(128) NOT NULL,
			    embedding VECTOR(1024)
			);
			
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_regions_name ON vehicles_module.regions(name);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу регионов.
	/// </summary>
	public override void Down()
	{
		Delete.Table("regions").InSchema("vehicles_module");
	}
}
