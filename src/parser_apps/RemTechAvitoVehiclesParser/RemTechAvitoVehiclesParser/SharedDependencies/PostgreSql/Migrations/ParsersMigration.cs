using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 6)]
public sealed class ParsersMigration : Migration
{
    public override void Up()
    {
        Create.Table("parsers")
            .InSchema("avito_parser_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("domain").AsString(128).NotNullable()
            .WithColumn("type").AsString(128).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("parsers").InSchema("avito_parser_module");
    }
}