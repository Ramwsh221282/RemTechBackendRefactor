using FluentMigrator;

namespace AvitoSparesParser.Database;

[Migration(1766848309)]
public sealed class ProcessingParserLinksMigration : Migration
{
    public override void Up()
    {
        Create.Table("processing_parser_links").InSchema("avito_spares_parser")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("processing_parser_links");
    }
}