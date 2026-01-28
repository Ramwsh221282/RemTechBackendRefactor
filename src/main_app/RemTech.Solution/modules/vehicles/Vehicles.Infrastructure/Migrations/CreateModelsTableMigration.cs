using FluentMigrator;

namespace Vehicles.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания таблицы моделей.
/// </summary>
[Migration(1767096661)]
public sealed class CreateModelsTableMigration : Migration
{
	/// <summary>
	/// Выполняет миграцию, создавая таблицу моделей.
	/// </summary>
	public override void Up() =>
		Execute.Sql(
			"""
			CREATE TABLE IF NOT EXISTS vehicles_module.models (
			    id UUID PRIMARY KEY,
			    name VARCHAR(255) NOT NULL,
			    embedding VECTOR(1024)
			);
			CREATE INDEX IF NOT EXISTS idx_models_embedding ON vehicles_module.models USING hnsw (embedding vector_cosine_ops);
			CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_models_name ON vehicles_module.models(name);
			"""
		);

	/// <summary>
	/// Откатывает миграцию, удаляя таблицу моделей.
	/// </summary>
	public override void Down() => Delete.Table("models").InSchema("vehicles_module");
}
