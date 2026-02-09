using System.Data;
using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы характеристик транспортных средств.
/// </summary>
[Migration(202501080009)]
public sealed class VehicleCharacteristicsMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу характеристик транспортных средств.
	/// </summary>
	public override void Up()
	{
		Create
			.Table("vehicle_characteristics")
			.InSchema("vehicles_module")
			.WithColumn("vehicle_id")
			.AsGuid()
			.ForeignKey("fk_vehicle_characteristics_vehicles", "vehicles_module", "vehicles", "id")
			.OnDelete(Rule.Cascade)
			.WithColumn("characteristic_id")
			.AsGuid()
			.ForeignKey("fk_vehicle_characteristics_characteristics", "vehicles_module", "characteristics", "id")
			.OnDelete(Rule.Cascade)
			.WithColumn("value")
			.AsString()
			.NotNullable()
			.WithColumn("name")
			.AsString()
			.NotNullable();
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу характеристик транспортных средств.
	/// </summary>
	public override void Down()
	{
		Delete.Table("vehicle_characteristics").InSchema("vehicles_module");
	}
}
