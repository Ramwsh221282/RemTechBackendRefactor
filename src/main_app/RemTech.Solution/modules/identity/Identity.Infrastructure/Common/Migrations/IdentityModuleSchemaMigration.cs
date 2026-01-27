using FluentMigrator;

namespace Identity.Infrastructure.Common.Migrations;

/// <summary>
/// Миграция для создания схемы модуля идентификации.
/// </summary>
[Migration(1767457100)]
public sealed class IdentityModuleSchemaMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая схему модуля идентификации.
	/// </summary>
	public override void Up() => Create.Schema("identity_module");

	/// <summary>
	/// Откатывает миграцию, удаляя схему модуля идентификации.
	/// </summary>
	public override void Down() => Delete.Schema("identity_module");
}
