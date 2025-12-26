using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 5)]
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