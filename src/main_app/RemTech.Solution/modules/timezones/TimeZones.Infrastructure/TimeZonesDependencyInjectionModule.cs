using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Timezones.Core.Contracts;
using Timezones.Core.Models;
using TimeZones.Infrastructure.Persistence;

namespace TimeZones.Infrastructure;

public static class TimeZonesDependencyInjectionModule
{
    extension(IServiceCollection services)
    {
        public void RegisterInfrastructure()
        {
            RegisterInfrastructureAdapters(services);            
            services.AddHostedService<RegionLocalDateTimesCacheUpdateService>();
        }
    }

    private static void RegisterInfrastructureAdapters(IServiceCollection services)
    {
        services.AddOptions<TimeZonesProviderOptions>().BindConfiguration(nameof(TimeZonesProviderOptions));
        services.TryAddSingleton<ITimeZonesProvider, TimeZonesProvider>();
        services.TryAddSingleton<IRegionDateTimesRepository, CachedRegionDateTimesRepository>();
    }    
}
