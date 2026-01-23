using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767027875)]
public sealed class BrandTableMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.brands (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			CREATE INDEX IF NOT EXISTS idx_brands_embedding ON vehicles_module.brands USING hnsw (embedding vector_cosine_ops);
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_brands_name ON vehicles_module.brands(name);
			"""
		);
	}

	public override void Down()
	{
		Delete.Table("brands").InSchema("vehicles_module");
	}
}
