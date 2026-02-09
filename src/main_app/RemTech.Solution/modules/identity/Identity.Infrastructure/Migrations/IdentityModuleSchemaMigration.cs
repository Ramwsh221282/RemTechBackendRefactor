using FluentMigrator;

namespace Identity.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания схемы модуля идентификации.
/// </summary>
[Migration(202501020000)]
public sealed class IdentityModuleSchemaMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая схему модуля идентификации.
	/// </summary>
	public override void Up()
	{
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
		Create.Schema("identity_module");
	}

	/// <summary>
	/// Откатывает миграцию, удаляя схему модуля идентификации.
	/// </summary>
	public override void Down()
	{
		Delete.Schema("identity_module");
	}
}
