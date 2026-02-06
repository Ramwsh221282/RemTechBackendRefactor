using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания индекса по значению цены в таблице транспортных средств.
/// </summary>
[Migration(1767278107)]
public sealed class VehiclesPriceValueIndex : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая индекс по значению цены в таблице транспортных средств.
	/// </summary>
	public override void Up()
	{
		Execute.Sql("CREATE INDEX IF NOT EXISTS idx_vehicles_price_value ON vehicles_module.vehicles (price);");
	}

	/// <summary>
	/// Откатывает миграцию, удаляя индекс по значению цены в таблице транспортных средств.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP INDEX IF EXISTS idx_vehicles_price_value;");
	}
}
