using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 8)]
public sealed class ItemsMigration : Migration
{
    public override void Up()
    {
        Create.Table("items")
            .InSchema("avito_parser_module")
            .WithColumn("id").AsString(64).PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("was_processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable()
            .WithColumn("price").AsInt64().NotNullable()
            .WithColumn("is_nds").AsBoolean().NotNullable()
            .WithColumn("address").AsString(512).NotNullable()
            .WithColumn("photos").AsCustom("jsonb").NotNullable()
            .WithColumn("title").AsString(512).Nullable()
            .WithColumn("characteristics").AsCustom("jsonb").Nullable();
    }

    public override void Down()
    {
        Delete.Table("items").InSchema("avito_parser_module");
    }
}