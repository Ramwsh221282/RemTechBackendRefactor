using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032642)]
public sealed class LocationTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("locations").InSchema("vehicles_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("embedding").AsCustom("vector(1024)").Nullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_locations_embedding ON vehicles_module.locations USING hnsw (embedding vector_cosine_ops)");
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_locations_name ON vehicles_module.locations(name)");
    }

    public override void Down()
    {
        Delete.Table("locations").InSchema("vehicles_module");
    }
}