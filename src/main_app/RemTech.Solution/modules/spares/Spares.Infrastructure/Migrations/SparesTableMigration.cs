using FluentMigrator;

namespace Spares.Infrastructure.Migrations;

/// <summary>
///     Миграция для создания таблицы запчастей.
/// </summary>
[Migration(1766981273)]
public sealed class SparesTableMigration : Migration
{
	/// <summary>
	///   Выполняет миграцию "вверх".
	/// </summary>
	public override void Up()
	{
		Create
			.Table("spares")
			.InSchema("spares_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey()
			.WithColumn("url")
			.AsString()
			.NotNullable()
			.WithColumn("price")
			.AsInt64()
			.NotNullable()
			.WithColumn("oem")
			.AsString(256)
			.NotNullable()
			.WithColumn("text")
			.AsString()
			.NotNullable()
			.WithColumn("is_nds")
			.AsBoolean()
			.NotNullable()
			.WithColumn("region_id")
			.AsGuid()
			.NotNullable()
			.WithColumn("type")
			.AsString(256)
			.NotNullable()
			.WithColumn("embedding")
			.AsCustom("vector(1024)")
			.Nullable();
		Execute.Sql("CREATE UNIQUE INDEX IF NOT EXISTS idx_spares_unique_url ON spares_module.spares(url)");
		Execute.Sql(
			"CREATE INDEX IF NOT EXISTS idx_spares_hnsw ON spares_module.spares USING hnsw (embedding vector_cosine_ops)"
		);
	}

	/// <summary>
	///  Выполняет миграцию "вниз".
	/// </summary>
	public override void Down()
	{
		Delete.Table("spares").InSchema("spares_module");
	}
}
