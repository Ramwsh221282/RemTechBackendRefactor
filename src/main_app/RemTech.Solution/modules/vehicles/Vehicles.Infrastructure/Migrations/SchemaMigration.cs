using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания схемы vehicles_module.
/// </summary>
[Migration(1767027778)]
public sealed class SchemaMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая схему vehicles_module.
	/// </summary>
	public override void Up()
	{
		Execute.Sql("CREATE SCHEMA IF NOT EXISTS vehicles_module;");
	}

	/// <summary>
	/// Откатывает миграцию, удаляя схему vehicles_module.
	/// </summary>
	public override void Down()
	{
		Delete.Schema("vehicles_module");
	}
}
