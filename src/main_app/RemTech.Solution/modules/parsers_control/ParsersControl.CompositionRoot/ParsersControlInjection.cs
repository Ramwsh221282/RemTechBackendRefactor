using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserWorkTurning.Contracts;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;
using ParsersControl.Infrastructure.ParserRegistrationManagement.NpgSql;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Infrastructure.ParserWorkTurning.ACL.RegisterDisabledParserOnParserRegistration;
using ParsersControl.Infrastructure.ParserWorkTurning.NpgSql;
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
            services.RegisterParserWorkTurningContext();
            services.RegisterParserStateManagementContext();
        }
        
        private void RegisterSharedDependencies()
        {
            services.AddScoped<IRegisteredParsersStorage, NpgSqlRegisteredParsersStorage>();
            services.AddScoped<NpgSqlRegisteredParsersStorage>();
            services.AddTransient<IDbUpgrader, ParsersControlModuleDbUpgrader>();
        }

        private void RegisterParserStateManagementContext()
        {
            services.AddScoped<LoggingOnStatefulParserStateChangeEventListener>();
            services.AddScoped<NpgSqlOnStatefulParserStateChangedEventListener>();
        }
        
        private void RegisterParserWorkTurningContext()
        {
            services.AddScoped<RegisterDisableParserOnParserRegistrationEventListener>();
            services.AddScoped<IParserWorkTurnersStorage, NpgSqlParserWorkTurnersStorage>();
        }
        
        private void RegisterParserRegistrationContext()
        {
            services.AddScoped<LoggingOnParserRegisteredEventListener>();
            services.AddScoped<NpgSqlOnParserRegisteredEventListener>();
            services.AddScoped<IGateway<AddParserRequest, AddParserResponse>, AddParserGateway>();
            
            services.Decorate<
                IGateway<AddParserRequest, AddParserResponse>,
                TransactionalGateway<AddParserRequest, AddParserResponse>>();
        }
    }
}