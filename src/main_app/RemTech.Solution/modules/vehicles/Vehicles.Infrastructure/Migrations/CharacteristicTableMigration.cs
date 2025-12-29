using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032660)]
public sealed class CharacteristicTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("characteristics").InSchema("vehicles_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("embedding").AsCustom("vector(1024)").Nullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_characteristics_embedding ON vehicles_module.characteristics USING hnsw (embedding vector_cosine_ops)");
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_characteristics_name ON vehicles_module.characteristics(name)");
    }

    public override void Down()
    {
        Delete.Table("characteristics").InSchema("vehicles_module");
    }
}