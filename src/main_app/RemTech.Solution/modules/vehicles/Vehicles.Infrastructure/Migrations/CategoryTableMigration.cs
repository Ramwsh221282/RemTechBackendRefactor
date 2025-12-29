using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032575)]
public sealed class CategoryTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("categories").InSchema("vehicles_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("embedding").AsCustom("vector(1024)").Nullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_categories_embedding ON vehicles_module.categories USING hnsw (embedding vector_cosine_ops)");
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_categories_name ON vehicles_module.categories(name)");
    }

    public override void Down()
    {
        Delete.Table("categories").InSchema("vehicles_module");
    }
}