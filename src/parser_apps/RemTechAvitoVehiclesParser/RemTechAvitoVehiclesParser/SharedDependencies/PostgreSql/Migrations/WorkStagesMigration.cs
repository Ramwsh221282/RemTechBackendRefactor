using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 5)]
public sealed class WorkStagesMigration : Migration
{
    public override void Up()
    {
        Create.Table("work_stages")
            .InSchema("avito_parser_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(128).NotNullable();
    }

    public override void Down()
    {
         Delete.Table("work_stages").InSchema("avito_parser_module");
    }    
}