using FluentMigrator;

namespace AvitoSparesParser.Database;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 2)]
public sealed class WorkStagesMigration : Migration
{
    public override void Up()
    {
        Create.Table("stages").InSchema("avito_spares_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(128).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("stages");
    }
}