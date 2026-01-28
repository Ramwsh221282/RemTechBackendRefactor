using FluentMigrator;

namespace Identity.Infrastructure.Accounts.Migrations;

/// <summary>
/// Миграция для создания таблицы аккаунтов.
/// </summary>
[Migration(1767457400)]
public sealed class AccountTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию вверх (создание таблицы аккаунтов).
	/// </summary>
	public override void Up() =>
		Create
			.Table("accounts")
			.InSchema("identity_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey()
			.NotNullable()
			.WithColumn("email")
			.AsString(256)
			.NotNullable()
			.WithColumn("password")
			.AsString()
			.NotNullable()
			.WithColumn("login")
			.AsString(256)
			.NotNullable()
			.WithColumn("activation_status")
			.AsBoolean()
			.NotNullable();

	/// <summary>
	/// Выполняет миграцию вниз (удаление таблицы аккаунтов).
	/// </summary>
	public override void Down() => Delete.Table("accounts").InSchema("identity_module");
}
