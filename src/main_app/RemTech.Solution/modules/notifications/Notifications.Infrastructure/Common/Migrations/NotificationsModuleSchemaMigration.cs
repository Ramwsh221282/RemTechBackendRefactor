using FluentMigrator;

namespace Notifications.Infrastructure.Common.Migrations;

[Migration(1767617100)]
public sealed class NotificationsModuleSchemaMigration : Migration
{
	public override void Up()
	{
		Execute.Sql("CREATE SCHEMA IF NOT EXISTS notifications_module;");
	}

	public override void Down()
	{
		Execute.Sql("DROP SCHEMA IF EXISTS notifications_module;");
	}
}
