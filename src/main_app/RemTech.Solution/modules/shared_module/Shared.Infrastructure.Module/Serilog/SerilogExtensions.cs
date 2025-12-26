using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Shared.Infrastructure.Module.Serilog;

public static class SerilogExtensions
{
    public static void AddSerilog(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton<ILogger>(logger);
    }
}
