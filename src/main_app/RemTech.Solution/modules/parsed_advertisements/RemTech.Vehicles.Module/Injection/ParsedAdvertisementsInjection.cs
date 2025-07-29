using Microsoft.Extensions.DependencyInjection;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;
using RemTech.Vehicles.Module.Features.SinkVehicles.Decorators.BackgroundService;

namespace RemTech.Vehicles.Module.Injection;

public static class ParsedAdvertisementsInjection
{
    public static void InjectParsedAdvertisementsModule(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundJobTransportAdvertisementSinking>();
    }

    public static async Task UpParsedAdvertisementsModuleDb(this IServiceProvider provider)
    {
        DatabaseConfiguration config = provider.GetRequiredService<DatabaseConfiguration>();
        DatabaseBakery bakery = new(config);
        bakery.Up(typeof(ParsedAdvertisementsInjection).Assembly);
    }
}
