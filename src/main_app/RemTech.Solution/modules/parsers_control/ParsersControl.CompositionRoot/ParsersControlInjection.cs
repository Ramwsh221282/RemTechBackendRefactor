using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserWorkTurning.Contracts;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;
using ParsersControl.Infrastructure.ParserRegistrationManagement.NpgSql;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Infrastructure.ParserStateManagement.NpgSql;
using ParsersControl.Infrastructure.ParserWorkTurning.ACL.RegisterDisabledParserOnParserRegistration;
using ParsersControl.Infrastructure.ParserWorkTurning.NpgSql;
using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using ParsersControl.Presenters.ParserStateManagement.Common;
using ParsersControl.Presenters.ParserStateManagement.Disable;
using ParsersControl.Presenters.ParserStateManagement.Enable;
using ParsersControl.Presenters.ParserStateManagement.PermanentDisable;
using ParsersControl.Presenters.ParserStateManagement.StartWaiting;
using ParsersControl.Presenters.ParserStateManagement.StartWorking;
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
            services.AddScoped<IOnStatefulParserStateChangedEventListener, NpgSqlOnStatefulParserStateChangedEventListener>();
            services.AddScoped<IOnStatefulParserStateChangedEventListener, LoggingOnStatefulParserStateChangeEventListener>();
            services.AddScoped<IStatefulParsersStorage, NpgSqlStatefulParsersStorage>();

            services.AddScoped<IGateway<DisableParserRequest, ParserStateChangeResponse>, DisableParserGateway>();
            services.AddScoped<IGateway<EnableParserGatewayRequest,  ParserStateChangeResponse>, EnableParserGateway>();
            services.AddScoped<IGateway<PermanentDisableRequest, ParserStateChangeResponse>, PermanentDisableGateway>();
            services.AddScoped<IGateway<StartWaitingRequest, ParserStateChangeResponse>, StartWaitingGateway>();
            services.AddScoped<IGateway<StartWorkingRequest,  ParserStateChangeResponse>, StartWorkingGateway>();
            
            services.Decorate<
                IGateway<DisableParserRequest, ParserStateChangeResponse>, 
                TransactionalGateway<DisableParserRequest, ParserStateChangeResponse>
            >();
            
            services.Decorate<
                IGateway<EnableParserGatewayRequest, ParserStateChangeResponse>, 
                TransactionalGateway<EnableParserGatewayRequest, ParserStateChangeResponse>
            >();
            
            services.Decorate<
                IGateway<PermanentDisableRequest, ParserStateChangeResponse>, 
                TransactionalGateway<PermanentDisableRequest, ParserStateChangeResponse>
            >();
            
            services.Decorate<
                IGateway<StartWaitingRequest, ParserStateChangeResponse>, 
                TransactionalGateway<StartWaitingRequest, ParserStateChangeResponse>
            >();
            
            services.Decorate<
                IGateway<StartWorkingRequest, ParserStateChangeResponse>, 
                TransactionalGateway<StartWorkingRequest, ParserStateChangeResponse>
            >();
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