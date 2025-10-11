using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remtech.Infrastructure.RabbitMQ.Consumers;

namespace Remtech.Infrastructure.RabbitMQ;

public static class RabbitMqExtensions
{
    public static void AddRabbitMqProvider(this IHostApplicationBuilder builder)
    {
        RabbitMqOptions? options = builder
            .Configuration.GetSection(nameof(RabbitMqOptions))
            .Get<RabbitMqOptions>();
        options.RegisterRabbitMqProvider(builder);
    }

    public static void AddRabbitMqProvider(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqConnectionProvider>();
    }

    public static void AddRabbitMqProvider(
        this IHostApplicationBuilder builder,
        string jsonFilePath
    )
    {
        if (!File.Exists(jsonFilePath))
            throw new InvalidOperationException(
                "JSON Configuration file was not found for rabbit mq options setup."
            );

        IConfigurationRoot root = builder.Configuration.AddJsonFile(jsonFilePath).Build();
        RabbitMqOptions? options = root.GetSection(nameof(RabbitMqOptions)).Get<RabbitMqOptions>();
        options.RegisterRabbitMqProvider(builder);
    }

    public static void AddRabbitMqProvider(
        this IHostApplicationBuilder builder,
        string jsonFilePath,
        Func<IConfigurationRoot, RabbitMqOptions> settingsProvider
    )
    {
        if (!File.Exists(jsonFilePath))
            throw new InvalidOperationException(
                "JSON Configuration file was not found for rabbit mq options setup."
            );

        IConfigurationRoot root = builder.Configuration.AddJsonFile(jsonFilePath).Build();
        RabbitMqOptions options = settingsProvider(root);
        options.RegisterRabbitMqProvider(builder);
    }

    private static void RegisterRabbitMqProvider(
        this RabbitMqOptions? options,
        IHostApplicationBuilder builder
    )
    {
        if (options == null)
            throw new InvalidOperationException("RabbitMqOptions were not set up.");
        if (string.IsNullOrWhiteSpace(options.HostName))
            throw new InvalidOperationException("RabbitMqOptions HostName was not set.");
        if (string.IsNullOrWhiteSpace(options.Port))
            throw new InvalidOperationException("RabbitMqOptions Port was not set.");
        if (string.IsNullOrWhiteSpace(options.Password))
            throw new InvalidOperationException("RabbitMqOptions Password was not set.");
        if (string.IsNullOrWhiteSpace(options.UserName))
            throw new InvalidOperationException("RabbitMqOptions Username was not set.");
        if (!int.TryParse(options.Port, out _))
            throw new InvalidOperationException("RabbitMqOptions Port was is incorrent value.");

        RabbitMqConnectionProvider provider = new RabbitMqConnectionProvider(options);
        builder.Services.AddSingleton(provider);
    }
}
