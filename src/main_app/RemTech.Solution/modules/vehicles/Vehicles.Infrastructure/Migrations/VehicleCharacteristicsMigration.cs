using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767097877)]
public sealed class VehicleCharacteristicsMigration : Migration
{
    public override void Up()
    {
        Create.Table("vehicle_characteristics").InSchema("vehicles_module")
            .WithColumn("vehicle_id").AsGuid().ForeignKey("fk_vehicle_characteristics_vehicles", "vehicles_module", "vehicles", "id")
            .WithColumn("characteristic_id").AsGuid().ForeignKey("fk_vehicle_characteristics_characteristics", "vehicles_module", "characteristics", "id")
            .WithColumn("value").AsString().NotNullable()
            .WithColumn("name").AsString().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("vehicle_characteristics").InSchema("vehicles_module");
    }    
}