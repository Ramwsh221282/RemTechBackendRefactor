using FluentMigrator;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Миграция для добавления расширения PgVector в базу данных PostgreSQL.
/// </summary>
[Migration(100)]
public sealed class PgVectorMigration : Migration
{
	/// <summary>
	/// Версия миграции.
	/// </summary>
	public const long VERSION = 20251201010100;

	/// <summary>
	/// Применяет миграцию, создавая расширение PgVector, если оно еще не существует.
	/// </summary>
	public override void Up()
	{
		Execute.Sql("CREATE EXTENSION IF NOT EXISTS vector;");
	}

	/// <summary>
	/// Откатывает миграцию, удаляя расширение PgVector, если оно существует.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP EXTENSION IF EXISTS vector;");
	}
}
