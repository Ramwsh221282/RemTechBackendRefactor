using FluentMigrator;

namespace Notifications.Infrastructure.Mailers.Migrations;

/// <summary>
/// Миграция для создания таблицы почтовых ящиков.
/// </summary>
[Migration(1767617200)]
public sealed class MailersTableMigration : Migration
{
	/// <summary>
	/// 	Выполняет миграцию вверх, создавая таблицу почтовых ящиков.
	/// </summary>
	public override void Up() =>
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS notifications_module.mailers
			(
			    id UUID NOT NULL PRIMARY KEY,
			    email VARCHAR(256) NOT NULL,
			    smtp_password TEXT NOT NULL
			)
			"""
		);

	/// <summary>
	/// 	Выполняет миграцию вниз, удаляя таблицу почтовых ящиков.
	/// </summary>
	public override void Down() => Execute.Sql("DROP TABLE IF EXISTS notifications_module.mailers;");
}
