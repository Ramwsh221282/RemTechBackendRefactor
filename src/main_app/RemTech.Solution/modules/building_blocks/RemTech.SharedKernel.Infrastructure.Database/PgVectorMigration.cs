using FluentMigrator;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Миграция для добавления расширения PgVector в базу данных PostgreSQL.
/// </summary>
[Migration(202501010000)]
public sealed class PgVectorMigration : Migration
{
	/// <summary>
	/// Применяет миграцию, создавая расширение PgVector, если оно еще не существует.
	/// </summary>
	public override void Up()
	{
		Execute.Sql("CREATE EXTENSION IF NOT EXISTS vector;");
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
	}

	/// <summary>
	/// Откатывает миграцию, удаляя расширение PgVector, если оно существует.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP EXTENSION IF EXISTS vector;");
        Execute.Sql("DROP EXTENSION IF EXISTS pg_trgm;");
	}
}
