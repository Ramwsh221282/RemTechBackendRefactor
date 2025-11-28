using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;
using ParsersControl.Infrastructure.ParserRegistrationManagement.NpgSql;
using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.CompositionRoot;

public static class ParsersControlInjection
{
    extension(IServiceCollection services)
    {
        public void AddParsersControlModule()
        {
            services.RegisterSharedDependencies();
            services.RegisterParserRegistrationContext();
        }

        private void RegisterSharedDependencies()
        {
            services.AddScoped<IRegisteredParsersStorage, NpgSqlRegisteredParsersStorage>();
            services.AddScoped<NpgSqlRegisteredParsersStorage>();
            services.AddTransient<IDbUpgrader, ParsersControlModuleDbUpgrader>();
        }

        private void RegisterParserRegistrationContext()
        {
            services.AddScoped<LoggingOnParserRegisteredEventListener>();
            services.AddScoped<NpgSqlOnParserRegisteredEventListener>();
            services.AddScoped<IGateway<AddParserRequest, AddParserResponse>, AddParserGateway>();
        }
    }
}