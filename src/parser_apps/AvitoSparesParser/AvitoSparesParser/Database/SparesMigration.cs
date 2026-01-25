using FluentMigrator;

namespace AvitoSparesParser.Database;

[Migration(1766848328)]
public sealed class SparesMigration : Migration
{
    public override void Up()
    {
        Create.Table("spares").InSchema("avito_spares_parser")
            .WithColumn("id").AsString(128).PrimaryKey()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("price").AsInt64().NotNullable()
            .WithColumn("is_nds").AsBoolean().NotNullable()
            .WithColumn("address").AsString().NotNullable()
            .WithColumn("photos").AsCustom("jsonb").NotNullable()
            .WithColumn("oem").AsString().NotNullable()
            .WithColumn("processed").AsBoolean().NotNullable()
            .WithColumn("retry_count").AsInt32().NotNullable()
            .WithColumn("type").AsString(128).Nullable()
            .WithColumn("title").AsString(256).Nullable();
    }

    public override void Down()
    {
        Delete.Table("spares");
    }
}