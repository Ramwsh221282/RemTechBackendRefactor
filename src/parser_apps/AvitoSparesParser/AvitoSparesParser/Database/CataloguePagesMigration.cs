using FluentMigrator;

namespace AvitoSparesParser.Database;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 5)]
public sealed class CataloguePagesMigration : Migration
{
    public override void Up()
    {
        Create.Table("catalogue_pages").InSchema("avito_spares_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("catalogue_pages");
    }
}