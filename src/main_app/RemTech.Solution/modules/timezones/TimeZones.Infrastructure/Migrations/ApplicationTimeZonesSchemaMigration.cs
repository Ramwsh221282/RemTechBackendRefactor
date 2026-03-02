using FluentMigrator;

namespace TimeZones.Infrastructure.Migrations;

[Migration(202501080018)]
public sealed class ApplicationTimeZonesSchemaMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP SCHEMA IF EXISTS timezones_module;");
    }

    public override void Up()
    {
        Execute.Sql("CREATE SCHEMA IF NOT EXISTS timezones_module;");
    }
}
