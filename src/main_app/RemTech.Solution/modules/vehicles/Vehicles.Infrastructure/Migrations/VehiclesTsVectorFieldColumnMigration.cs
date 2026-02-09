using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для добавления столбца ts_vector_field в таблицу транспортных средств.
/// </summary>
[Migration(202501080010)]
public sealed class VehiclesTsVectorFieldColumnMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, добавляя столбец ts_vector_field в таблицу транспортных средств.
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			"""
			ALTER TABLE vehicles_module.vehicles 
			    ADD COLUMN IF NOT EXISTS ts_vector_field tsvector 
			        GENERATED ALWAYS AS (to_tsvector('russian', text)) STORED; 
			"""
		);

		Execute.Sql(
			"CREATE INDEX IF NOT EXISTS idx_vehicles_ts_vector ON vehicles_module.vehicles USING gin(ts_vector_field);"
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя столбец ts_vector_field из таблицы транспортных средств.
	/// </summary>
	public override void Down()
	{
		Execute.Sql(
			"""
			ALTER TABLE vehicles_module.vehicles 
			    DROP COLUMN IF EXISTS ts_vector_field;
			"""
		);
		Execute.Sql("DROP INDEX IF EXISTS idx_vehicles_ts_vector;");
	}
}
