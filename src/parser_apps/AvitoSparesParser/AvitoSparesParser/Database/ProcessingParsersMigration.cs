using FluentMigrator;

namespace AvitoSparesParser.Database;

[Migration(1766848301)]
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