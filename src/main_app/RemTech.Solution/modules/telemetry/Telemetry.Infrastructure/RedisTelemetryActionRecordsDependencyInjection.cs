using Microsoft.Extensions.DependencyInjection;

namespace Telemetry.Infrastructure;

/// <summary>
/// Регистрация зависимостей для работы с записями действий телеметрии в Redis.
/// </summary>
public static class RedisTelemetryActionRecordsDependencyInjection
{
	extension(IServiceCollection services)
	{
		public void RegisterRedisActionRecordDependencies()
		{
			services.AddSingleton<RedisTelemetryActionsStorage>();
			services.AddHostedService<ActionRecordsPersistingBackgroundProcess>();
		}
	}
}
