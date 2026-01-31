using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы городов.
/// </summary>
[Migration(1767086200)]
public sealed class LocationCitiesMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу городов.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.cities (
			    id UUID PRIMARY KEY,
			    name VARCHAR(256) NOT NULL,
			    embedding VECTOR(1024)
			);
			CREATE INDEX IF NOT EXISTS idx_cities_embedding ON vehicles_module.cities USING hnsw (embedding vector_l2_ops);
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_cities_name ON vehicles_module.cities(name);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу городов.
	/// </summary>
	public override void Down()
	{
		Delete.Table("cities").InSchema("vehicles_module");
	}
}
