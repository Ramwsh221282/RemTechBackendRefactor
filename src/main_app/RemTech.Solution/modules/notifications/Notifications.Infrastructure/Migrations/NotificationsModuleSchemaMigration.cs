using FluentMigrator;

namespace Notifications.Infrastructure.Migrations;

/// <summary>
/// Миграция схемы модуля уведомлений.
/// </summary>
[Migration(202501040000)]
public sealed class NotificationsModuleSchemaMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию вверх.
	/// </summary>
	public override void Up()
	{
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
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
