using FluentMigrator;

namespace ContainedItems.Infrastructure.Migrations;

[Migration(1767202462)]
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