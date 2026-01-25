using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766859098)]
public sealed class WorkingParserLinks : Migration
{
    public override void Up()
    {
        Create.Table("working_parser_links")
            .InSchema("drom_vehicles_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("working_parser_links")
            .InSchema("drom_vehicles_parser");
    }
}