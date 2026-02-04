using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

[Migration(1770229700)]
public sealed class SparesTypesMigration : Migration
{
	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS spares_module.types;");
	}

	public override void Up()
	{
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS spares_module.types
			(
				id UUID PRIMARY KEY,
				type VARCHAR(256) NOT NULL,				
				embedding VECTOR(1024),
				UNIQUE (type)
			);

			CREATE INDEX IF NOT EXISTS idx_spares_type ON spares_module.types(type);
			CREATE INDEX IF NOT EXISTS idx_spares_type_trgm ON spares_module.types USING GIN (type gin_trgm_ops);
			CREATE INDEX IF NOT EXISTS idx_spares_hnsw_type ON spares_module.types USING hnsw (embedding vector_cosine_ops);
			"""
		);
	}
}
