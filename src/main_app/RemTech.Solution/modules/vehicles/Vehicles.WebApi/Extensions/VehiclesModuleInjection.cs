using System.Reflection;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.NN;
using Vehicles.Domain.Brands.Contracts;
using Vehicles.Domain.Categories.Contracts;
using Vehicles.Domain.Characteristics.Contracts;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Features.AddVehicle;
using Vehicles.Domain.Locations.Contracts;
using Vehicles.Domain.Models.Contracts;
using Vehicles.Domain.Vehicles;
using Vehicles.Domain.Vehicles.Contracts;
using Vehicles.Infrastructure.BackgroundServices;
using Vehicles.Infrastructure.Brands.BrandsPersisterImplementation;
using Vehicles.Infrastructure.Categories.CategoriesPersisterImplementation;
using Vehicles.Infrastructure.Characteristics.CharacteristicsPersisterImplementation;
using Vehicles.Infrastructure.CommonImplementations;
using Vehicles.Infrastructure.Locations.LocationsPersisterImplementation;
using Vehicles.Infrastructure.Models.ModelPersisterImplementation;
using Vehicles.Infrastructure.RabbitMq.Producers;
using Vehicles.Infrastructure.Vehicles.PersisterImplementation;
using Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

namespace Vehicles.WebApi.Extensions;

public static class VehiclesModuleInjection
{
    extension(IServiceCollection services)
    {
        public void InjectVehiclesModule()
        {
            services.RegisterDomainLayerDependencies();
            services.RegisterInfrastructureLayerDependencies();
        }
        
        public void RegisterVehiclesModule(bool isDevelopment)
        {
            services.RegisterSharedInfrastructure(isDevelopment);
            services.RegisterDomainLayerDependencies();
            services.RegisterInfrastructureLayerDependencies();
        }
    
        private void RegisterDomainLayerDependencies()
        {
            new HandlersRegistrator(services)
                .FromAssemblies([typeof(Vehicle).Assembly])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .RequireRegistrationOf(typeof(IEventTransporter<,>))
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
        }

        public void RegisterInfrastructureLayerDependencies()
        {
            Assembly assembly = typeof(NpgSqlVehiclesPersister).Assembly;
            services.RegisterQueryHandlers(assembly);
            services.RegisterConsumers(assembly);
            services.RegisterPersisters();
            services.RegisterBackgroundServices();
            services.RegisterProducers();
        }

        private void RegisterQueryHandlers(Assembly assembly)
        {
            new HandlersRegistrator(services)
                .FromAssemblies([assembly])
                .RequireRegistrationOf(typeof(IQueryHandler<,>))
                .Invoke();
        }
        
        private void RegisterMigrations(Assembly assembly)
        {
            services.AddMigrations([assembly]);
        }
        
        private void RegisterConsumers(Assembly assembly)
        {
            services.Scan(x => x.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IConsumer)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
        }

        private void RegisterProducers()
        {
            services.AddSingleton<IOnVehiclesAddedEventPublisher, OnVehiclesAddedProducer>();
        }
        
        private void RegisterPersisters()
        {
            services.AddScoped<IVehiclesListPersister, NpgSqlVehiclesListPersister>();
            services.AddScoped<IBrandPersister, NpgSqlBrandPersisterImplementation>();
            services.AddScoped<IModelsPersister, NpgSqlModelPersister>();
            services.AddScoped<ICategoryPersister, NpgSqlCategoriesPersisterImplementation>();
            services.AddScoped<ICharacteristicsPersister, NpgSqlCharacteristicsPersister>();
            services.AddScoped<ILocationsPersister, NpgSqlLocationsPersister>();
            services.AddScoped<IVehiclesPersister, NpgSqlVehiclesPersister>();
            services.AddScoped<IPersister, NpgSqlPersister>();
        }

        private void RegisterBackgroundServices()
        {
            services.AddHostedService<VehicleEmbeddingsUpdaterService>();
        }
        
        private void RegisterSharedInfrastructure(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                Assembly assembly = typeof(NpgSqlVehiclesPersister).Assembly;
                services.RegisterMigrations(assembly);
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
                services.RegisterFromAppsettings();
                services.AddOptions<GetVehiclesThresholdConstants>().BindConfiguration(nameof(GetVehiclesThresholdConstants));
            }
            
            services.AddSingleton<EmbeddingsProvider>();
            services.AddPostgres();
            services.AddRabbitMq();
        }
    }
}