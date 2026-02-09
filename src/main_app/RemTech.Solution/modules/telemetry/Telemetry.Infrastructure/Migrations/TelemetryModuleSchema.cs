using FluentMigrator;

namespace Telemetry.Infrastructure.Migrations;

/// <summary>
/// Миграция для создания схемы телеметрии.
/// </summary>
[Migration(202501070000)]
public sealed class TelemetryModuleSchema : Migration
{
	/// <summary>
	/// Применяет миграцию.
	/// </summary>
	public override void Up()
	{
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");
		Execute.Sql("CREATE SCHEMA IF NOT EXISTS telemetry_module;");
	}

	/// <summary>
	/// Откатывает миграцию.
	/// </summary>
	public override void Down()
	{
		Execute.Sql("DROP SCHEMA IF EXISTS telemetry_module;");
	}
}
