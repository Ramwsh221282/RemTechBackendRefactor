using FluentMigrator;

namespace Identity.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы тикетов модуля identity.
/// </summary>
[Migration(202501020003)]
public sealed class TicketTableMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая таблицу тикетов модуля identity.
	/// </summary>
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

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу тикетов модуля identity.
	/// </summary>
	public override void Down()
	{
		Delete.Table("tickets").InSchema("identity_module");
	}
}
