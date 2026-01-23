using FluentMigrator;

namespace Identity.Infrastructure.Permissions.Migrations;

[Migration(1767457700)]
public sealed class PredefinedPermissionsSeeding : Migration
{
    public override void Up() =>
        Execute.EmbeddedScript("Identity.Infrastructure.Permissions.Seeding.predefined-permissions-seeding.sql");

    public override void Down() { }
}
