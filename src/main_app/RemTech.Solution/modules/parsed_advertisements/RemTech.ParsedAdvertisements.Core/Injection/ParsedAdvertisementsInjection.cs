using Microsoft.Extensions.DependencyInjection;
using RemTech.ParsedAdvertisements.Core.Features.SinkVehicles.Decorators.BackgroundService;
using RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

namespace RemTech.ParsedAdvertisements.Core.Injection;

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
