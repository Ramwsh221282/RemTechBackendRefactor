using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[Migration(1766811837)]
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