using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

[Migration(1767277993)]
public sealed class VehiclesTsVectorFieldColumnMigration : Migration
{
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
