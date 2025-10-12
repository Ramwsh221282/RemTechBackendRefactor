using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RemTech.Infrastructure.PostgreSQL;

public static class NpgSqlOptionsExtensions
{
    public static void AddNpgSqlOptions(this IHostApplicationBuilder builder) =>
        builder.Configuration.AddNpgSqlOptions(builder.Services);

    public static void AddNpgSqlOptions(
        this IConfigurationManager manager,
        IServiceCollection services
    )
    {
        NpgsqlOptions? options = manager.GetSection(nameof(NpgsqlOptions)).Get<NpgsqlOptions>();
        if (options == null)
            throw new ArgumentException($"{nameof(NpgsqlOptions)} is required in appsettings.json");
        services.AddNpgSqlOptions(options);
    }

    public static void AddNpgSqlOptions(this IServiceCollection services, NpgsqlOptions options) =>
        services.AddSingleton(options);
}
