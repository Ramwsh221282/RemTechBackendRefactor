using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[Migration(1766858985)]
public sealed class WorkStagesMigration : Migration
{
    public override void Up()
    {
        Create.Table("work_stages")
            .InSchema("drom_vehicles_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("stage_name").AsString(64).NotNullable()
            .WithColumn("finished").AsBoolean().NotNullable();        
    }

    public override void Down()
    {
        Delete.Table("work_stages")
            .InSchema("drom_vehicles_parser");
    }
}