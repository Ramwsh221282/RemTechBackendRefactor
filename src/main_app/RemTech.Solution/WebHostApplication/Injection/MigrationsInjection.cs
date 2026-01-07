using System.Reflection;
using ContainedItems.Infrastructure.Repositories;
using Identity.Infrastructure.Accounts;
using Notifications.Infrastructure.Mailers;
using ParsersControl.Infrastructure.Parsers.Repository;
using RemTech.SharedKernel.Infrastructure.Database;
using Spares.Infrastructure.Repository;
using Vehicles.Infrastructure.BackgroundServices;

namespace WebHostApplication.Injection;

public static class MigrationsInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterModuleMigrations()
        {
            Assembly[] assemblies =
            [
                typeof(ContainedItemsRepository).Assembly,
                typeof(AccountsRepository).Assembly,
                typeof(MailersRepository).Assembly,
                typeof(SubscribedParsersRepository).Assembly,
                typeof(SparesRepository).Assembly,
                typeof(VehicleEmbeddingsUpdaterService).Assembly
            ];
            
            services.AddMigrations(assemblies);
        }
    }
}