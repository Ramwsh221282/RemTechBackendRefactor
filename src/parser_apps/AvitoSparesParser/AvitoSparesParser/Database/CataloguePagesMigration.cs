using FluentMigrator;

namespace AvitoSparesParser.Database;

[Migration(1766848319)]
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