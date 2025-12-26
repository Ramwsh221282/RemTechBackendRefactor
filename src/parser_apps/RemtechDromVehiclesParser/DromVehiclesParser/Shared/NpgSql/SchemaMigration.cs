using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 1)]
public sealed class SchemaMigration : Migration
{
    public override void Up()
    {
        Create.Schema("drom_vehicles_parser");
    }

    public override void Down()
    {
        Delete.Schema("drom_vehicles_parser");
    }
}