using FluentMigrator;

namespace Identity.Infrastructure.Accounts.Migrations;

/// <summary>
/// Миграция для создания таблицы разрешений аккаунтов.
/// </summary>
[Migration(1767457500)]
public sealed class AccountPermissionsMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию вверх (создание таблицы разрешений аккаунтов).
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS identity_module.account_permissions (
			    account_id UUID NOT NULL,
			    permission_id UUID NOT NULL,
			    PRIMARY KEY (account_id, permission_id),
			    FOREIGN KEY (account_id) REFERENCES identity_module.accounts (id),
			    FOREIGN KEY (permission_id) REFERENCES identity_module.permissions (id)
			);
			"""
		);
	}

	/// <summary>
	/// Выполняет миграцию вниз (удаление таблицы разрешений аккаунтов).
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS identity_module.account_permissions;");
	}
}
