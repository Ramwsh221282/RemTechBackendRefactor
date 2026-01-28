using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы транспортных средств.
/// </summary>
[Migration(1767097795)]
public sealed class VehiclesTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу транспортных средств.
	/// </summary>
	public override void Up()
	{
		Create
			.Table("vehicles")
			.InSchema("vehicles_module")
			.WithColumn("id")
			.AsGuid()
			.PrimaryKey("pk_vehicles_id")
			.WithColumn("brand_id")
			.AsGuid()
			.ForeignKey("fk_vehicles_brands", "vehicles_module", "brands", "id")
			.WithColumn("category_id")
			.AsGuid()
			.ForeignKey("fk_vehicles_categories", "vehicles_module", "categories", "id")
			.WithColumn("model_id")
			.AsGuid()
			.ForeignKey("fk_vehicles_models", "vehicles_module", "models", "id")
			.WithColumn("region_id")
			.AsGuid()
			.ForeignKey("fk_vehicles_regions", "vehicles_module", "regions", "id")
			.WithColumn("source")
			.AsString()
			.NotNullable()
			.WithColumn("price")
			.AsInt64()
			.NotNullable()
			.WithColumn("is_nds")
			.AsBoolean()
			.NotNullable()
			.WithColumn("text")
			.AsString()
			.NotNullable()
			.WithColumn("photos")
			.AsCustom("jsonb")
			.NotNullable()
			.WithColumn("characteristics")
			.AsCustom("jsonb")
			.NotNullable()
			.WithColumn("embedding")
			.AsCustom("vector(1024)")
			.Nullable();
		Execute.Sql(
			"CREATE INDEX IF NOT EXISTS idx_vehicles_hnsw ON vehicles_module.vehicles USING hnsw (embedding vector_cosine_ops)"
		);
	}

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу транспортных средств.
	/// </summary>
	public override void Down() => Delete.Table("vehicles").InSchema("vehicles_module");
}
