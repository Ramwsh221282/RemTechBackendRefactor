using FluentMigrator;

namespace RemTechAvitoVehiclesParser.SharedDependencies.PostgreSql.Migrations;

[Migration(1766811890)]
public sealed class ParserLinksMigration : Migration
{
    public override void Up()
    {
        Create.Table("parser_links")
            .InSchema("avito_parser_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("was_processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("parser_links").InSchema("avito_parser_module");
    }
}