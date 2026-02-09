using FluentMigrator;

namespace Identity.Infrastructure.Migrations;

/// <summary>
/// Миграция для добавления предопределённых разрешений в таблицу permissions модуля identity.
/// </summary>
[Migration(202501020008)]
public sealed class PredefinedPermissionsSeeding : Migration
{
	/// <summary>
	/// Применяет миграцию, добавляя предопределённые разрешения в таблицу permissions модуля identity.
	/// </summary>
	public override void Up()
	{
		Execute.EmbeddedScript("Identity.Infrastructure.Migrations.predefined-permissions-seeding.sql");
	}

	/// <summary>
	/// Откатывает миграцию. В данном случае не выполняет никаких действий.
	/// </summary>
	public override void Down() { }
}
