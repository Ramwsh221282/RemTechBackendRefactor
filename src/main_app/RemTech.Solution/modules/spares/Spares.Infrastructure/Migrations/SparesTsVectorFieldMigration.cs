using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

/// <summary>
///    Миграция для добавления поля tsvector в таблицу запчастей.
/// </summary>
[Migration(1767356828)]
public sealed class SparesTsVectorFieldMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию "вверх".
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			ALTER TABLE spares_module.spares
			ADD COLUMN IF NOT EXISTS ts_vector_field tsvector
			GENERATED ALWAYS AS (to_tsvector('russian', (text || ' ' || type || ' ' || oem)))
			STORED;
			"""
		);

		Execute.Sql(
			"CREATE INDEX IF NOT EXISTS idx_spares_ts_vector ON spares_module.spares USING gin(ts_vector_field);"
		);
	}

	/// <summary>
	/// Выполняет миграцию "вниз".
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP INDEX IF EXISTS idx_spares_ts_vector;");
		Execute.Sql(
			"""
			ALTER TABLE spares_module.spares
			DROP COLUMN IF EXISTS ts_vector_field;
			"""
		);
	}
}
