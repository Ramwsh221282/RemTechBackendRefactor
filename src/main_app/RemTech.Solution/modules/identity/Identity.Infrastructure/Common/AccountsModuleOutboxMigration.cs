using FluentMigrator;

namespace Identity.Infrastructure.Common;

/// <summary>
/// Миграция для создания таблицы outbox модуля аккаунтов.
/// </summary>
[Migration(1767457600)]
public sealed class AccountsModuleOutboxMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая таблицу outbox модуля аккаунтов.
	/// </summary>
	public override void Up() =>
		Create
			.Table("outbox")
			.InSchema("identity_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey()
			.WithColumn("type")
			.AsString(128)
			.NotNullable()
			.WithColumn("retry_count")
			.AsInt32()
			.NotNullable()
			.WithColumn("created")
			.AsDateTime()
			.NotNullable()
			.WithColumn("sent")
			.AsDateTime()
			.Nullable()
			.WithColumn("payload")
			.AsCustom("jsonb")
			.NotNullable();

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу outbox модуля аккаунтов.
	/// </summary>
	public override void Down() => Delete.Table("outbox").InSchema("identity_module");
}
