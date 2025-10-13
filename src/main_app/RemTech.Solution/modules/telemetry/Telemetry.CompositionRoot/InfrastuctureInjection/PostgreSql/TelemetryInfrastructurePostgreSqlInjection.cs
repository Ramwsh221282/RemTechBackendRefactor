using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using RemTech.Infrastructure.PostgreSQL;
using Telemetry.Domain.TelemetryContext.Contracts;
using Telemetry.Infrastructure.PostgreSQL.Repositories;

namespace Telemetry.CompositionRoot.InfrastuctureInjection.PostgreSql;

/// <summary>
/// Инъекция работы с PostgreSQL у модуля телеметрии
/// </summary>
[InjectionClass]
public static class TelemetryInfrastructurePostgreSqlInjection
{
    [InjectionMethod]
    public static void InjectTelemetryPostgreSql(this IServiceCollection services)
    {
        services.InjectEmbeddingGenerator();
        services.AddScoped<TelemetryServiceDbContext>();
        services.AddScoped<ITelemetryRecordsRepository, TelemetryRecordsRepository>();
    }
}
