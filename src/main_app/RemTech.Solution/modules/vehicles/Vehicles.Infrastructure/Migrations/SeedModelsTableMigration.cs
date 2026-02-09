using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для начального заполнения таблицы моделей.
/// </summary>
[Migration(202501080013)]
public sealed class SeedModelsTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, заполняя таблицу моделей начальными данными.
	/// </summary>
	public override void Up()
	{
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0002.models_seeding_1.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0003.models_seeding_2.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0004.models_seeding_3.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0005.models_seeding_4.sql");
		Execute.EmbeddedScript("Vehicles.Infrastructure.Models.SeedingImplementation.0006.models_seeding_5.sql");
	}

	/// <summary>
	/// Откатывает миграцию. В данном случае откат не выполняется.
	/// </summary>
	public override void Down() { }
}
