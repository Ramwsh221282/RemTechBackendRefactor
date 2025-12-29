using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767032601)]
public sealed class ModelTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("models").InSchema("vehicles_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("name").AsString(255).NotNullable()
            .WithColumn("embedding").AsCustom("vector(1024)").Nullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_models_embedding ON vehicles_module.models USING hnsw (embedding vector_cosine_ops)");
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_models_name ON vehicles_module.models(name)");
    }

    public override void Down()
    {
        Delete.Table("models").InSchema("vehicles_module");
    }
}