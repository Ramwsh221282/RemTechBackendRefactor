using FluentMigrator;

namespace Telemetry.Infrastructure;

/// <summary>
/// Миграция для создания схемы телеметрии.
/// </summary>
[Migration(1769587891)]
public sealed class TelemetryModuleSchema : Migration
{
	/// <summary>
	/// Применяет миграцию.
	/// </summary>
	public override void Up()
	{
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
