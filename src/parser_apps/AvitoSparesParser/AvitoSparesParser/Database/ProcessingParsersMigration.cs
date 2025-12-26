using FluentMigrator;

namespace AvitoSparesParser.Database;

[TimestampedMigration(year: 2025, month: 12, day: 5, hour: 5, minute: 3)]
public sealed class ProcessingParsersMigration : Migration
{
    public override void Up()
    {
        Create.Table("processing_parsers").InSchema("avito_spares_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("domain").AsString(128).NotNullable()
            .WithColumn("type").AsString(128).NotNullable()
            .WithColumn("finished").AsDateTimeOffset().Nullable()
            .WithColumn("entered").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("processing_parsers");
    }
}