using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

[Migration(202501060001)]
public sealed class SparesOemMigration : Migration
{
	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS spares_module.oems;");
	}

	public override void Up()
	{
		Execute.Sql(
			""" 
			CREATE TABLE IF NOT EXISTS spares_module.oems
			(
				id UUID PRIMARY KEY,
				oem VARCHAR(64) NOT NULL,
				embedding VECTOR(1024),
				UNIQUE (oem)
			);

			CREATE INDEX IF NOT EXISTS idx_spares_oem ON spares_module.oems(oem);
			CREATE INDEX IF NOT EXISTS idx_spares_oem_trgm ON spares_module.oems USING GIN (oem gin_trgm_ops);
			"""
		);
	}
}
