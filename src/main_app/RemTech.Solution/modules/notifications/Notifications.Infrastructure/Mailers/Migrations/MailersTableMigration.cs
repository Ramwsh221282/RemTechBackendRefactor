using FluentMigrator;

namespace Notifications.Infrastructure.Mailers.Migrations;

[Migration(1767617200)]
public sealed class MailersTableMigration : Migration
{
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

	public override void Down() => Execute.Sql("DROP TABLE IF EXISTS notifications_module.mailers;");
}
