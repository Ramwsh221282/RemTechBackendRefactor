using Microsoft.Extensions.DependencyInjection;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Messaging;

namespace ParsedAdvertisements.Adapters.Messaging;

public static class DependencyInjection
{
    public static void AddParsedAdvertisementsMessagingAdapter(this IServiceCollection services)
    {
        services.AddTransient<IVehicleCreatedEventPublisher, VehicleCreatedEventPublisher>();
    }
}