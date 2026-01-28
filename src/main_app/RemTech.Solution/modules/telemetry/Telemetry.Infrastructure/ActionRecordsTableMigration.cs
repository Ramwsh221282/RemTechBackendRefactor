using FluentMigrator;

namespace Telemetry.Infrastructure;

/// <summary>
/// Миграция для создания таблицы записей действий телеметрии.
/// </summary>
[Migration(1769588891)]
public sealed class ActionRecordsTableMigration : Migration
{
	/// <summary>
	/// Применяет миграцию.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS telemetry_module.action_records
			(
			    id UUID PRIMARY KEY,
				invoker_id UUID,
			    name VARCHAR(255) NOT NULL,
			    severity VARCHAR(50) NOT NULL,
			    error TEXT NULL,
				payload JSONB,
			    created_at TIMESTAMPTZ NOT NULL,
			    embedding vector(1024),
			    ts_vector_field tsvector GENERATED ALWAYS AS
			        (to_tsvector('russian', coalesce(name, '') || ' ' || coalesce(error, '')))
			        STORED
			);
			"""
		);
	}

	/// <summary>
	/// Откатывает миграцию.
	/// </summary>
	public override void Down()
	{
		Execute.Sql(
			"""
			DROP TABLE IF EXISTS telemetry_module.action_records;
			"""
		);
	}
}
