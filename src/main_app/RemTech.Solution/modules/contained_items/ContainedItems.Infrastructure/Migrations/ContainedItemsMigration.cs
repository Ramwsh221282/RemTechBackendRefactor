using FluentMigrator;

namespace ContainedItems.Infrastructure.Migrations;

public sealed class ContainedItemsTypeMigration : Migration
{
    public override void Up()
    {
        Alter.Table("contained_items").InSchema("contained_items_module")
            .AddColumn("item_type").AsString(128).Nullable();
    }

    public override void Down()
    {
        Delete.Column("item_type").FromTable("contained_items").InSchema("contained_items_module");
    }
}

[Migration(1766906341)]
public sealed class ContainedItemsMigration : Migration
{
    public override void Up()
    {
        Create.Schema("contained_items_module");
        Create.Table("contained_items").InSchema("contained_items_module")
            .WithColumn("id").AsGuid().PrimaryKey()
            .WithColumn("service_item_id").AsString(255).NotNullable()
            .WithColumn("creator_id").AsGuid().NotNullable()
            .WithColumn("creator_type").AsString(128).NotNullable()
            .WithColumn("creator_domain").AsString(128).NotNullable()
            .WithColumn("content").AsCustom("jsonb").NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("deleted_at").AsDateTime().Nullable()
            .WithColumn("status").AsString(128).NotNullable();
        Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_contained_items_service_item_id ON contained_items_module.contained_items(service_item_id)");
    }

    public override void Down()
    {
        Delete.Table("contained_items").InSchema("contained_items_module");
        Delete.Schema("contained_items_module");        
        Execute.Sql("DROP INDEX IF EXISTS idx_contained_items_service_item_id");
    }
}