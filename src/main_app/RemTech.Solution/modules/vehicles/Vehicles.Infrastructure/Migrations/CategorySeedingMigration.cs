using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для начального заполнения таблицы категорий.
/// </summary>
[Migration(1767073579)]
public sealed class CategorySeedingMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, заполняя таблицу категорий начальными данными.
	/// </summary>
	public override void Up() =>
		Execute.EmbeddedScript(
			"Vehicles.Infrastructure.Categories.SeedingImplementation.Scripts.0002.categories_seeding.sql"
		);

	/// <summary>
	/// Откатывает миграцию. В данном случае откат не выполняется.
	/// </summary>
	public override void Down() { }
}
