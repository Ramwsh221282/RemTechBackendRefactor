using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767027875)]
public sealed class BrandTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("brands").InSchema("vehicles_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("embedding").AsCustom("vector(1024)").Nullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_brands_embedding ON vehicles_module.brands USING hnsw (embedding vector_cosine_ops)");
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_brands_name ON vehicles_module.brands(name)");
    }

    public override void Down()
    {
        Delete.Table("brands").InSchema("vehicles_module");
    }
}