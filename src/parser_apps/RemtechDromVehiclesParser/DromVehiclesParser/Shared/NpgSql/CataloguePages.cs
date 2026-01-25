using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766859113)]
public sealed class CataloguePages : Migration
{
    public override void Up()
    {
        Create.Table("catalogue_pages")
            .InSchema("drom_vehicles_parser")
            .WithColumn("url").AsString().PrimaryKey()
            .WithColumn("processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("catalogue_pages")
            .InSchema("drom_vehicles_parser");
    }
}