using FluentMigrator;

namespace Notifications.Infrastructure.Common.Migrations;

/// <summary>
/// Миграция схемы модуля уведомлений.
/// </summary>
[Migration(1767617100)]
public sealed class NotificationsModuleSchemaMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию вверх.
	/// </summary>
	public override void Up()
	{
		Execute.Sql("CREATE SCHEMA IF NOT EXISTS notifications_module;");
	}

	/// <summary>
	/// Выполняет миграцию вниз.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP SCHEMA IF EXISTS notifications_module;");
	}
}
