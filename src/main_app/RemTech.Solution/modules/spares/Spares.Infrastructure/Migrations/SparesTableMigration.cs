using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

[Migration(1766981273)]
public sealed class SparesTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("spares").InSchema("spares_module")
            .WithColumn("id").AsString(255).PrimaryKey()
            .WithColumn("contained_item_id").AsGuid().NotNullable()
            .WithColumn("url").AsString().NotNullable()
            .WithColumn("content").AsCustom("jsonb").NotNullable()
            .WithColumn("embedding").AsCustom("vector(1024)").Nullable();
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_spares_unique_url ON spares_module.spares(url)");
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_spares_hnsw ON spares_module.spares USING hnsw (embedding vector_cosine_ops)");
        Execute.Sql("CREATE INDEX idx_spares_oem ON spares_module.spares ((content->>'oem'))");
        Execute.Sql("CREATE INDEX idx_spares_type ON spares_module.spares ((content->>'type'))");
        Execute.Sql("CREATE INDEX idx_spares_address ON spares_module.spares ((content->>'address'))");
        Execute.Sql("CREATE INDEX idx_spares_is_nds ON spares_module.spares ((content->'is_nds'))");
        Execute.Sql("CREATE INDEX idx_spares_price ON spares_module.spares ((content->>'price'))");
        Execute.Sql("CREATE INDEX idx_spares_price_numeric ON spares_module.spares ((content->>'price'))");
    }

    public override void Down() => Delete.Table("spares").InSchema("spares_module");
}