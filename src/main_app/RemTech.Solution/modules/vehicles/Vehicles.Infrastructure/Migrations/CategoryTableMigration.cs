using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы категорий.
/// </summary>
[Migration(202501080002)]
public sealed class CategoryTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу категорий.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.categories (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_categories_name ON vehicles_module.categories(name);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу категорий.
	/// </summary>
	public override void Down()
	{
		Delete.Table("categories").InSchema("vehicles_module");
	}
}
