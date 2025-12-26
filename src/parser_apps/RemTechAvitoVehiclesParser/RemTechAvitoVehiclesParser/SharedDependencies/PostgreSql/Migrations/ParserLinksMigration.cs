using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 7)]
public sealed class ParserLinksMigration : Migration
{
    public override void Up()
    {
        Create.Table("parser_links")
            .InSchema("avito_parser_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("was_processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("parser_links").InSchema("avito_parser_module");
    }
}