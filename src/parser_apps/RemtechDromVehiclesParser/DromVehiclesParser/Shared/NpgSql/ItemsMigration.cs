using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766859106)]
public sealed class ItemsMigration : Migration
{
    public override void Up()
    {
        Create.Table("items")
            .InSchema("drom_vehicles_parser")
            .WithColumn("id").AsString(128).PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("photos").AsCustom("jsonb").NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable()
            .WithColumn("processed").AsBoolean().NotNullable()
            .WithColumn("characteristics").AsCustom("jsonb").Nullable()
            .WithColumn("price").AsInt64().Nullable()
            .WithColumn("is_nds").AsBoolean().Nullable()
            .WithColumn("title").AsString().Nullable()
            .WithColumn("address").AsString().Nullable();
    }

    public override void Down()
    {
        Delete.Table("items")
            .InSchema("drom_vehicles_parser");
    }
}