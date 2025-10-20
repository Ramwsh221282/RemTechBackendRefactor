using GeoLocations.Module.Features.Querying;
using GeoLocations.Module.OnStartup;
using Microsoft.Extensions.DependencyInjection;

namespace GeoLocations.Module.Injection;

public static class GeoLocationsModuleInjection
{
    public static void InjectLocationsModule(this IServiceCollection services)
    {
        services.AddHostedService<LocationsSeeding>();
        services.AddSingleton<IGeoLocationQueryService, GeoLocationsQueryService>();
    }
}
