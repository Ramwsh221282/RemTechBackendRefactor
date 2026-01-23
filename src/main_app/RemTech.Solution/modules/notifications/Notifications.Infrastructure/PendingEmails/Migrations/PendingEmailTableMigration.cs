using FluentMigrator;

namespace Notifications.Infrastructure.PendingEmails.Migrations;

[Migration(1767617300)]
public sealed class PendingEmailTableMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS notifications_module.pending_emails
			(
			    id uuid primary key,
			    recipient varchar(256) not null,
			    subject varchar(256) not null,
			    body text not null,
			    was_sent boolean not null
			)
			"""
		);
	}

	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS notifications_module.pending_emails");
	}
}
