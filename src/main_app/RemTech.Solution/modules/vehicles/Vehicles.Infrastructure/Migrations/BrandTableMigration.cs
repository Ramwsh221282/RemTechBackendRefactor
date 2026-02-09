using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы брендов.
/// </summary>
[Migration(202501080001)]
public sealed class BrandTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу брендов.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.brands (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_brands_name ON vehicles_module.brands(name);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу брендов.
	/// </summary>
	public override void Down()
	{
		Delete.Table("brands").InSchema("vehicles_module");
	}
}
