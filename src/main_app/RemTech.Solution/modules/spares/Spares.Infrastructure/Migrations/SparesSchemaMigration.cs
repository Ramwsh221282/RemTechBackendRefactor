using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

/// <summary>
///     Миграция для создания схемы модуля запчастей.
/// </summary>
[Migration(1766980872)]
public sealed class SparesSchemaMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию "вверх".
	/// </summary>
	public override void Up()
	{
		Create.Schema("spares_module");
	}

	/// <summary>
	/// Выполняет миграцию "вниз".
	/// </summary>
	public override void Down()
	{
		Delete.Schema("spares_module");
	}
}
