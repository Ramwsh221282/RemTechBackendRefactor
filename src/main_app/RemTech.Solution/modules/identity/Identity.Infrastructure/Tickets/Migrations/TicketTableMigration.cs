using FluentMigrator;

namespace Identity.Infrastructure.Tickets.Migrations;

[Migration(1767457300)]
public sealed class TicketTableMigration : Migration
{
	public override void Up()
	{
		Create
			.Table("tickets")
			.InSchema("identity_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey()
			.NotNullable()
			.WithColumn("creator_id")
			.AsGuid()
			.NotNullable()
			.WithColumn("finished")
			.AsBoolean()
			.NotNullable()
			.WithColumn("purpose")
			.AsString(256)
			.NotNullable();
		Execute.Sql("CREATE INDEX IF NOT EXISTS idx_tickets_creator_id ON identity_module.tickets(creator_id)");
	}

	public override void Down() => Delete.Table("tickets").InSchema("identity_module");
}
