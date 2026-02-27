using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Core.Handlers;
using Timezones.Core.Contracts;
using Timezones.Core.Models;

namespace TimeZones.Infrastructure;

public static class TimeZonesDependencyInjectionModule
{
    extension(IServiceCollection services)
    {
        public void InjectTimeZonesModule()
        {                    
            services.AddOptions<TimeZonesProviderOptions>().BindConfiguration(nameof(TimeZonesProviderOptions));            
            services.TryAddSingleton<ITimeZonesProvider, TimeZonesProvider>();
            services.TryAddSingleton<ITimeZonesRepository, CachedTimeZonesRepository>();
            services.TryAddScoped<IQueryHandler<GetTimeZonesQuery, IReadOnlyList<TimeZoneRecord>>, GetTimeZonesQueryHandler>();
            services.AddHostedService<TimeZoneRecordsCacheUpdateService>();
        }
    }
}
