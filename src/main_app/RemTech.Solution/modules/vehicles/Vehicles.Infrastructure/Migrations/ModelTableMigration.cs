using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы моделей.
/// </summary>
[Migration(202501080004)]
public sealed class ModelTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу моделей.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.models (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_models_name ON vehicles_module.models(name);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу моделей.
	/// </summary>
	public override void Down()
	{
		Delete.Table("models").InSchema("vehicles_module");
	}
}
