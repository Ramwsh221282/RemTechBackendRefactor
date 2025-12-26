using FluentMigrator;

namespace AvitoSparesParser.Database;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 1)]
public sealed class SchemaMigrations : Migration
{
    public override void Up()
    {
        Create.Schema("avito_spares_parser");
    }

    public override void Down()
    {
        Delete.Schema("avito_spares_parser");
    }
}