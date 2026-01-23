using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032660)]
public sealed class CharacteristicTableMigration : Migration
{
	public override void Up() =>
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.characteristics (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			CREATE INDEX IF NOT EXISTS idx_characteristics_embedding ON vehicles_module.characteristics USING hnsw (embedding vector_cosine_ops);
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_characteristics_name ON vehicles_module.characteristics(name);
			"""
		);

	public override void Down() => Delete.Table("characteristics").InSchema("vehicles_module");
}
