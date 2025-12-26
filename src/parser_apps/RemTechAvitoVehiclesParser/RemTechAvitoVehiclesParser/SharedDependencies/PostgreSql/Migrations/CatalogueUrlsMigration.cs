using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 9)]
public sealed class CatalogueUrlsMigration : Migration
{
    public override void Up()
    {
        Create.Table("catalogue_urls")
            .InSchema("avito_parser_module")
            .WithColumn("url").AsString().PrimaryKey()
            .WithColumn("was_processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("catalogue_urls").InSchema("avito_parser_module");
    }
}