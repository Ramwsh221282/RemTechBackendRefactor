using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032642)]
public sealed class LocationTableMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                     CREATE TABLE IF NOT EXISTS vehicles_module.locations (
                         id UUID PRIMARY KEY,
                         name VARCHAR(255) NOT NULL,
                         embedding VECTOR(1024)
                     );
                     CREATE INDEX IF NOT EXISTS idx_locations_embedding ON vehicles_module.locations USING hnsw (embedding vector_cosine_ops);
                     CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_locations_name ON vehicles_module.locations(name);
                     """);
    }

    public override void Down()
    {
        Delete.Table("locations").InSchema("vehicles_module");
    }
}