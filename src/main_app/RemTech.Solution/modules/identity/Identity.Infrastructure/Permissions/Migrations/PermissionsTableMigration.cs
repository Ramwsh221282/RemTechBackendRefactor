using FluentMigrator;

namespace Identity.Infrastructure.Permissions.Migrations;

[Migration(1767457605)]
public sealed class PermissionsTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("permissions").InSchema("identity_module")
            .WithColumn("id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("name").AsString(256).NotNullable()
            .WithColumn("description").AsString(256).NotNullable();
        Execute.Sql("CREATE INDEX IF NOT EXISTS idx_permissions_name ON identity_module.permissions(name)");
    }

    public override void Down()
    {
        Delete.Table("permissions").InSchema("identity_module");
    }
}