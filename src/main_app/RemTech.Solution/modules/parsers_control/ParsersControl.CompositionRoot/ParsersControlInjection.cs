using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core;
using ParsersControl.Core.Contracts;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.Parsers.Repository;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.CompositionRoot;

public static class ParsersControlInjection
{
    extension(IServiceCollection services)
    {
        public void AddParsersControlModule()
        {
            services.RegisterPersistence();
            services.RegisterUseCaseHandler();
        }
        
        private void RegisterPersistence()
        {
            services.AddTransient<IDbUpgrader, ParsersControlModuleDbUpgrader>();
            services.AddScoped<ISubscribedParsersRepository, SubscribedParsersRepository>();
        }

        private void RegisterUseCaseHandler()
        {
            services.RegisterParserControlHandlers();
        }
    }
}