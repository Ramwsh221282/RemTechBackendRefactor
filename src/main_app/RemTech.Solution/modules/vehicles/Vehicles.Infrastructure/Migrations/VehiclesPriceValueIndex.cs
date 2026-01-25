using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767278107)]
public sealed class VehiclesPriceValueIndex : Migration
{
    public override void Up() =>
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_vehicles_price_value ON vehicles_module.vehicles (price);");

    public override void Down() => Execute.Sql("DROP INDEX IF EXISTS idx_vehicles_price_value;");
}
