using FluentMigrator;

namespace ParsersControl.Infrastructure.Migrations;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 5)]
public sealed class ParsersTableMigration : Migration
{
    public override void Up()
    {
        Create.Schema("parsers_control_module");
        Create.Table("registered_parsers").InSchema("parsers_control_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("type").AsString(128).NotNullable()
            .WithColumn("domain").AsString(128).NotNullable()
            .WithColumn("state").AsString(128).NotNullable()
            .WithColumn("processed").AsInt32().NotNullable()
            .WithColumn("elapsed_seconds").AsInt64().NotNullable()
            .WithColumn("started_at").AsDateTimeOffset().Nullable()
            .WithColumn("finished_at").AsDateTimeOffset().Nullable()
            .WithColumn("next_run").AsDateTimeOffset().Nullable()
            .WithColumn("wait_days").AsInt32().Nullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_registered_parsers_type ON parsers_control_module.registered_parsers(type)");
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_registered_parsers_domain ON parsers_control_module.registered_parsers(domain)");
    }

    public override void Down()
    {
        Execute.Sql("DROP INDEX IF EXISTS idx_registered_parsers_type");
        Execute.Sql("DROP INDEX IF EXISTS idx_registered_parsers_domain");
        Delete.Table("registered_parsers").InSchema("parsers_control_module");
        Execute.Sql("DROP SCHEMA IF EXISTS parsers_control_module");
    }
}