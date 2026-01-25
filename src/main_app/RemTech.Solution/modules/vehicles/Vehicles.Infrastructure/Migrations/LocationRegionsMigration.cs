using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767086100)]
public sealed class LocationRegionsMigration : Migration
{
	public override void Up() =>
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.regions (
			    id UUID PRIMARY KEY,
			    name VARCHAR(256) NOT NULL,
			    kind VARCHAR(128) NOT NULL,
			    embedding VECTOR(1024)
			);
			CREATE INDEX IF NOT EXISTS idx_regions_embedding ON vehicles_module.regions USING hnsw (embedding vector_l2_ops);
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_regions_name ON vehicles_module.regions(name);
			"""
		);

	public override void Down() => Delete.Table("regions").InSchema("vehicles_module");
}
