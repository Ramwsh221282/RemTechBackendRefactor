using FluentMigrator;

namespace Identity.Infrastructure.Permissions.Migrations;

/// <summary>
/// Миграция для создания таблицы permissions модуля identity.
/// </summary>
[Migration(1767457200)]
public sealed class PermissionsTableMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая таблицу permissions модуля identity.
	/// </summary>
	public override void Up()
	{
		Create
			.Table("permissions")
			.InSchema("identity_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey()
			.NotNullable()
			.WithColumn("name")
			.AsString(256)
			.NotNullable()
			.WithColumn("description")
			.AsString(256)
			.NotNullable();
		Execute.Sql("CREATE INDEX IF NOT EXISTS idx_permissions_name ON identity_module.permissions(name)");
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу permissions модуля identity.
	/// </summary>
	public override void Down() => Delete.Table("permissions").InSchema("identity_module");
}
