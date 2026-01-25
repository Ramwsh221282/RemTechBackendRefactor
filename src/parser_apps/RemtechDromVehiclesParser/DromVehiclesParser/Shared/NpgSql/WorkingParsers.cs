using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766859086)]
public sealed class WorkingParsers : Migration
{
    public override void Up()
    {
        Create.Table("working_parsers")
            .InSchema("drom_vehicles_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("domain").AsString(128).NotNullable()
            .WithColumn("type").AsString(128).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("working_parsers")
            .InSchema("drom_vehicles_parser");
    }
}