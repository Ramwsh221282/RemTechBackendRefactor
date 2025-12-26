using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 4)]
public class SchemaMigration : Migration
{
    public override void Up()
    {
        Create.Schema("avito_parser_module");
    }

    public override void Down()
    {
        Delete.Schema("avito_parser_module");
    }
}