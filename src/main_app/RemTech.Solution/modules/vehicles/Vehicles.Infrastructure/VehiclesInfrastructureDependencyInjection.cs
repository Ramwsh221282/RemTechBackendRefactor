using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using Vehicles.Domain.Brands.Contracts;
using Vehicles.Domain.Categories.Contracts;
using Vehicles.Domain.Characteristics.Contracts;
using Vehicles.Domain.Contracts;
using Vehicles.Domain.Locations.Contracts;
using Vehicles.Domain.Models.Contracts;
using Vehicles.Infrastructure.Brands.BrandsPersisterImplementation;
using Vehicles.Infrastructure.Categories.CategoriesPersisterImplementation;
using Vehicles.Infrastructure.Characteristics.CharacteristicsPersisterImplementation;
using Vehicles.Infrastructure.CommonImplementations;
using Vehicles.Infrastructure.Locations.LocationsPersisterImplementation;
using Vehicles.Infrastructure.Models.ModelPersisterImplementation;

namespace Vehicles.Infrastructure;

public static class VehiclesInfrastructureDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterVehiclesModuleInfrastructure()
        {
            services.RegisterPersister();
            services.RegisterConsumers();
            services.RegisterMigrations();
        }

        private void RegisterMigrations()
        {
            Assembly assembly = typeof(VehiclesInfrastructureDependencyInjection).Assembly;
            services.AddMigrations([assembly]);
        }
        
        private void RegisterConsumers()
        {
            Assembly assembly = typeof(VehiclesInfrastructureDependencyInjection).Assembly;
            services.Scan(x => x.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IConsumer)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
        }
        
        private void RegisterPersister()
        {
            services.AddScoped<IBrandPersister, NpgSqlBrandPersisterImplementation>();
            services.AddScoped<IModelsPersister, NpgSqlModelPersister>();
            services.AddScoped<ICategoryPersister, NpgSqlCategoriesPersisterImplementation>();
            services.AddScoped<ICharacteristicsPersister, NpgSqlCharacteristicsPersister>();
            services.AddScoped<ILocationsPersister, NpgSqlLocationsPersister>();
            services.AddScoped<IPersister, NpgSqlPersister>();
        }
    }
}