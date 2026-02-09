using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

/// <summary>
///     Миграция для создания таблицы запчастей.
/// </summary>
[Migration(202501060003)]
public sealed class SparesTableMigration : Migration
{
	/// <summary>
	///   Выполняет миграцию "вверх".
	/// </summary>
	public override void Up()
	{
		Execute.Sql(
			""" 
			CREATE TABLE IF NOT EXISTS spares_module.spares
			(
				id UUID PRIMARY KEY,
				oem_id UUID NOT NULL REFERENCES spares_module.oems(id),
				type_id UUID NOT NULL REFERENCES spares_module.types(id),				
				url TEXT NOT NULL,
				price BIGINT NOT NULL,				
				text TEXT NOT NULL,
				is_nds BOOLEAN NOT NULL,
				region_id UUID NOT NULL,				
				ts_vector_field tsvector NOT NULL,
				photos jsonb,
				embedding VECTOR(1024)
			);

			CREATE UNIQUE INDEX IF NOT EXISTS idx_spares_unique_url ON spares_module.spares(url);
			CREATE INDEX IF NOT EXISTS idx_spares_price ON spares_module.spares(price);
			CREATE INDEX IF NOT EXISTS idx_spares_region_id ON spares_module.spares(region_id);
			CREATE INDEX IF NOT EXISTS idx_spares_trgm ON spares_module.spares USING GIN (text gin_trgm_ops);
			CREATE INDEX IF NOT EXISTS idx_spares_tsvector ON spares_module.spares USING GIN (ts_vector_field);			
			"""
		);
	}

	/// <summary>
	///  Выполняет миграцию "вниз".
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS spares_module.spares;");
	}
}
