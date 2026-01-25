using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766858979)]
public sealed class SchemaMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE SCHEMA IF NOT EXISTS drom_vehicles_parser;");
    }

    public override void Down()
    {
        Delete.Schema("drom_vehicles_parser");
    }
}