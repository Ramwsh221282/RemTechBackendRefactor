using FluentMigrator;

namespace AvitoSparesParser.Database;

[Migration(1766848286)]
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