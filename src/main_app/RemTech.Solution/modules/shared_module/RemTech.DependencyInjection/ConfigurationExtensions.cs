using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RemTech.DependencyInjection;

public static class ConfigurationExtensions
{
    public static void RegisterFromJsonRoot<T>(this IHostApplicationBuilder builder)
        where T : class
    {
        string sectionName = typeof(T).Name;
        T? configuration = builder.Configuration.GetSection(sectionName).Get<T>();
        if (configuration == null)
            throw new ApplicationException(
                $"Configuration section from root {sectionName} cannot be found"
            );
        builder.Services.AddSingleton(configuration);
    }
}
