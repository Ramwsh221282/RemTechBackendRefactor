using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы характеристик.
/// </summary>
[Migration(1767032660)]
public sealed class CharacteristicTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу характеристик.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.characteristics (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_characteristics_name ON vehicles_module.characteristics(name);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу характеристик.
	/// </summary>
	public override void Down()
	{
		Delete.Table("characteristics").InSchema("vehicles_module");
	}
}
