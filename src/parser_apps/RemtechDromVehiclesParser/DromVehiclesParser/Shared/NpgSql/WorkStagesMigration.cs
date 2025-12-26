using FluentMigrator;

namespace DromVehiclesParser.Shared.NpgSql;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 2)]
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