using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RemTech.DependencyInjection;
using RemTech.Infrastructure.PostgreSQL;
using Remtech.Infrastructure.RabbitMQ;
using Serilog;

namespace Vehicles.CompositionRoot;

public static class VehiclesModuleInjection
{
    public static void InjectVehiclesModule(
        this IHostApplicationBuilder builder,
        IEnumerable<Assembly> assemblies
    )
    {
        builder.AddConfiguration();
        builder.AddAspNetCoreDependencies();
        builder.AddLogger();
        builder.Services.RegisterModuleServices(assemblies, "Vehicles");
    }

    private static void AddAspNetCoreDependencies(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
    }

    private static void AddLogger(this IHostApplicationBuilder builder)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        builder.Services.AddSingleton(logger);
    }

    private static void AddConfiguration(this IHostApplicationBuilder builder)
    {
        builder.RegisterFromJsonRoot<RabbitMqOptions>();
        builder.RegisterFromJsonRoot<NpgsqlOptions>();
    }
}
