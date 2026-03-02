using FluentMigrator;

namespace TimeZones.Infrastructure.Migrations;

[Migration(202501080019)]
public sealed class ApplicationTimeZoneTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS timezones_module.selected_timezone;");
    }

    public override void Up()
    {
        Execute.Sql(
            """
            CREATE TABLE IF NOT EXISTS timezones_module.selected_timezone
            (
                region_name VARCHAR(255) NOT NULL PRIMARY KEY,
                offset_seconds BIGINT NOT NULL,
                timestamp_seconds BIGINT NOT NULL,
                year INT NOT NULL,
                month INT NOT NULL,
                day INT NOT NULL,
                hour INT NOT NULL,
                minute INT NOT NULL,
                second INT NOT NULL
            );
            """
            );
    }
}