using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для начального заполнения характеристик транспортных средств.
/// </summary>
[Migration(1767097485)]
public sealed class CharacteristicsSeeding : Migration
{
	/// <summary>
	/// Выполняет миграцию, заполняя таблицу характеристик начальными данными.
	/// </summary>
	public override void Up()
	{
		Execute.EmbeddedScript(
			"Vehicles.Infrastructure.Characteristics.SeedingImplementation.characteristics_seeding.sql"
		);
	}

	/// <summary>
	/// Откатывает миграцию. В данном случае откат не выполняется.
	/// </summary>
	public override void Down() { }
}
