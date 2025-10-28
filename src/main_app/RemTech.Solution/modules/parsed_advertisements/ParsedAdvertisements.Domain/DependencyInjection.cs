using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ParsedAdvertisements.Domain.BrandContext.Ports;
using ParsedAdvertisements.Domain.CategoryContext.Ports;
using ParsedAdvertisements.Domain.CharacteristicContext.Ports;
using ParsedAdvertisements.Domain.Common.UseCases.AddVehicle;
using ParsedAdvertisements.Domain.Common.UseCases.AddVehicle.Decorators;
using ParsedAdvertisements.Domain.ModelContext.Ports;
using ParsedAdvertisements.Domain.RegionContext.Ports;
using ParsedAdvertisements.Domain.VehicleContext;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Domain;

public static class DependencyInjection
{
    public static void AddParsedAdvertisementsDomain(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddHandlersFromAssembly(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.ReconfigureAddVehicleHandler();
    }

    private static void ReconfigureAddVehicleHandler(this IServiceCollection services)
    {
        services.RemoveAll<ICommandHandler<AddVehicleCommand, Status<Vehicle>>>();
        services.RemoveAll<AddVehicleCommandHandler>();
        services.RemoveAll<AddVehicleLoggingHandler>();
        services.RemoveAll<AddVehicleTransactionalHandler>();
        services.RemoveAll<AddVehicleValidationalHandler>();
        services.AddScoped<ICommandHandler<AddVehicleCommand, Status<Vehicle>>>(sp =>
        {
            var logger = sp.GetRequiredService<Serilog.ILogger>();
            var txnManager = sp.GetRequiredService<ITransactionManager>();
            var validator = sp.GetRequiredService<IValidator<AddVehicleCommand>>();
            var outbox = sp.GetRequiredService<IParsedAdvertisementsOutboxDeliverer>();
            var categories = sp.GetRequiredService<ICategoriesStorage>();
            var brands = sp.GetRequiredService<IBrandsStorage>();
            var models = sp.GetRequiredService<IModelsStorage>();
            var regions = sp.GetRequiredService<IRegionsStorage>();
            var characteristics = sp.GetRequiredService<ICharacteristicsStorage>();
            var vehicles = sp.GetRequiredService<IVehiclesStorage>();
            var origin = new AddVehicleCommandHandler(
                txnManager,
                brands,
                categories,
                regions,
                models,
                characteristics,
                vehicles);
            var transactional = new AddVehicleTransactionalHandler(origin, outbox, txnManager);
            var validating = new AddVehicleValidationalHandler(transactional, validator);
            var logging = new AddVehicleLoggingHandler(validating, logger);
            return logging;
        });
    }
}