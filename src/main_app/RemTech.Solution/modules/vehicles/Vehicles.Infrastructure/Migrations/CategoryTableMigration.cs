using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032575)]
public sealed class CategoryTableMigration : Migration
{
    public override void Up() => Execute.Sql(
            """
			CREATE TABLE IF NOT EXISTS vehicles_module.categories (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			CREATE INDEX IF NOT EXISTS idx_categories_embedding ON vehicles_module.categories USING hnsw (embedding vector_cosine_ops);
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_categories_name ON vehicles_module.categories(name);
			"""
        );

    public override void Down() => Delete.Table("categories").InSchema("vehicles_module");
}
