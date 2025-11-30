using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserScheduleManagement.Contracts;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkIgnoredListener;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkParserAttachedListener;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkRenamedListener;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUnignoredListener;
using ParsersControl.Infrastructure.ParserLinkManagement.Listeners.ParserLinkUrlChangedListener;
using ParsersControl.Infrastructure.ParserLinkManagement.NpgSql;
using ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;
using ParsersControl.Infrastructure.ParserRegistrationManagement.NpgSql;
using ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnCreated;
using ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnParserCreated;
using ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnUpdated;
using ParsersControl.Infrastructure.ParserScheduleManagement.NpgSql;
using ParsersControl.Infrastructure.ParserStateManagement.EventListeners;
using ParsersControl.Infrastructure.ParserStateManagement.NpgSql;
using ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnCreated;
using ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnParserRegistered;
using ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnUpdated;
using ParsersControl.Infrastructure.ParserStatistics.NpgSql;
using ParsersControl.Infrastructure.ParserWorkTurning.ACL.RegisterDisabledParserOnParserRegistration;
using ParsersControl.Infrastructure.ParserWorkTurning.NpgSql;
using ParsersControl.Presenters.ParserLinkManagement.AttachParserLink;
using ParsersControl.Presenters.ParserLinkManagement.ChangeUrlParserLink;
using ParsersControl.Presenters.ParserLinkManagement.Common;
using ParsersControl.Presenters.ParserLinkManagement.IgnoreParserLink;
using ParsersControl.Presenters.ParserLinkManagement.RenameParserLink;
using ParsersControl.Presenters.ParserLinkManagement.UnignoreParserLink;
using ParsersControl.Presenters.ParserRegistrationManagement.AddParser;
using ParsersControl.Presenters.ParserScheduleManagement.Common;
using ParsersControl.Presenters.ParserScheduleManagement.SetFinishedDate;
using ParsersControl.Presenters.ParserScheduleManagement.UpdateWaitDays;
using ParsersControl.Presenters.ParserStateManagement.Common;
using ParsersControl.Presenters.ParserStateManagement.Disable;
using ParsersControl.Presenters.ParserStateManagement.Enable;
using ParsersControl.Presenters.ParserStateManagement.PermanentDisable;
using ParsersControl.Presenters.ParserStateManagement.StartWaiting;
using ParsersControl.Presenters.ParserStateManagement.StartWorking;
using ParsersControl.Presenters.ParserStatisticsManagement.Common;
using ParsersControl.Presenters.ParserStatisticsManagement.UpdateElapsedSeconds;
using ParsersControl.Presenters.ParserStatisticsManagement.UpdateProcessed;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
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
            services.RegisterParserLinksManagementContext();
            services.RegisterParserStatisticsManagementContext();
            services.RegisterParserScheduleManagementContext();
        }
        
        private void RegisterSharedDependencies()
        {
            services.AddScoped<IRegisteredParsersStorage, NpgSqlRegisteredParsersStorage>();
            services.AddScoped<NpgSqlRegisteredParsersStorage>();
            services.AddTransient<IDbUpgrader, ParsersControlModuleDbUpgrader>();
        }

        private void RegisterParserStatisticsManagementContext()
        {
            services.AddScoped<IParserStatisticsCreatedEventListener, NpgSqlOnParserStatisticsCreatedEventListener>();
            services.AddScoped<IParserStatisticsCreatedEventListener, LoggingOnParserStatisticsCreatedEventListener>();

            services.AddScoped<IParserStatisticsUpdatedEventListener, NpgSqlOnParserStatisticsUpdatedEventListener>();
            services.AddScoped<IParserStatisticsUpdatedEventListener, LoggingOnParserStatisticsUpdatedEventListener>();

            services.AddScoped<NpgSqlOnParserStatisticsCreatedEventListener>();
            services.AddScoped<LoggingOnParserStatisticsCreatedEventListener>();
            services.AddScoped<NpgSqlOnParserStatisticsUpdatedEventListener>();
            services.AddScoped<LoggingOnParserStatisticsUpdatedEventListener>();
            services.AddScoped<RegisterEmptyStatisticsOnParserRegisteredListener>();
            services.AddScoped<IParserStatisticsStorage, NpgSqlParserStatisticsStorage>();
            
            services.AddScoped<
                IGateway<UpdateParserElapsedSecondsRequest, ParserStatisticsUpdateResponse>,
                UpdateParserElapsedSecondsGateway>();
            
            services.AddScoped<
                IGateway<UpdateParserProcessedRequest, ParserStatisticsUpdateResponse>,
                UpdateParserProcessedGateway>();
            
            services.Decorate<
                IGateway<UpdateParserElapsedSecondsRequest, ParserStatisticsUpdateResponse>,
                TransactionalGateway<UpdateParserElapsedSecondsRequest, ParserStatisticsUpdateResponse>
            >();
            
            services.Decorate<
                IGateway<UpdateParserProcessedRequest, ParserStatisticsUpdateResponse>,
                TransactionalGateway<UpdateParserProcessedRequest, ParserStatisticsUpdateResponse>
            >();
        }

        private void RegisterParserScheduleManagementContext()
        {
            services.AddScoped<IParserScheduleStorage, NpgSqlParserSchedulesStorage>();
            services.AddScoped<IParserScheduleCreatedEventListener, LoggingParserScheduleCreatedEventListener>();
            services.AddScoped<IParserScheduleCreatedEventListener, NpgSqlParserScheduleCreatedListener>();
            services.AddScoped<IParserScheduleUpdatedEventListener, LoggingParserScheduleUpdatedListener>();
            services.AddScoped<IParserScheduleUpdatedEventListener, NpgSqlParserScheduleUpdatedListener>();
            
            services.AddScoped<NpgSqlParserSchedulesStorage>();
            services.AddScoped<LoggingParserScheduleCreatedEventListener>();
            services.AddScoped<NpgSqlParserScheduleCreatedListener>();
            services.AddScoped<LoggingParserScheduleUpdatedListener>();
            services.AddScoped<NpgSqlParserScheduleUpdatedListener>();
            
            services.AddScoped<AddScheduleForParserOnParserCreatedEventListener>();
            
            services.AddScoped<
                IGateway<UpdateWaitDaysRequest, ParserScheduleUpdateResponse>,
                UpdateWaitDaysGateway>();

            services.AddScoped<
                IGateway<SetFinishedDateRequest, ParserScheduleUpdateResponse>,
                SetFinishedDateGateway
            >();

            services.Decorate<
                IGateway<UpdateWaitDaysRequest, ParserScheduleUpdateResponse>,
                TransactionalGateway<UpdateWaitDaysRequest, ParserScheduleUpdateResponse>
            >();
            
            services.Decorate<
                IGateway<SetFinishedDateRequest, ParserScheduleUpdateResponse>,
                TransactionalGateway<SetFinishedDateRequest, ParserScheduleUpdateResponse>
            >();
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

        private void RegisterParserLinksManagementContext()
        {
            services.AddScoped<IParserLinksStorage, NpgSqlParserLinksStorage>();
            
            services.AddScoped<IParserLinkIgnoredListener, LoggingParserLinkIgnoredListener>();
            services.AddScoped<IParserLinkIgnoredListener, NpgSqlParserLinkIgnoredListener>();

            services.AddScoped<IParserLinkParserAttached, LoggingParserLinkAttachedListener>();
            services.AddScoped<IParserLinkParserAttached, NpgSqlParserLinkAttachedListener>();

            services.AddScoped<IParserLinkRenamedListener, LoggingParserLinkRenamedListener>();
            services.AddScoped<IParserLinkRenamedListener, NpgSqlParserLinkRenamedListener>();

            services.AddScoped<IParserLinkUnignoredListener, LoggingParserLinkUnignoredListener>();
            services.AddScoped<IParserLinkUnignoredListener, NpgSqlParserLinkUnignoredListener>();

            services.AddScoped<IParserLinkUrlChangedListener, LoggingParserLinkUrlChangedListener>();
            services.AddScoped<IParserLinkUrlChangedListener, NpgSqlParserLinkUrlChangedListener>();

            services.AddScoped<FetchParserLinkById>(sp =>
            {
                IParserLinksStorage storage = sp.Resolve<IParserLinksStorage>();
                return async (id, ct) =>
                {
                    ParserLinkQueryArgs query = new(Id: id);
                    ParserLink? link = await storage.Fetch(query, ct);
                    if (link == null) return Error.NotFound("Ссылка парсера не найдена.");
                    return link;
                };
            });
            
            services.AddScoped<IGateway<AttachParserLinkRequest, ParserLinkResponse>, AttachParserLinkGateway>();
            services.AddScoped<IGateway<ChangeParserLinkRequest, ParserLinkResponse>, ChangeParserLinkGateway>();
            services.AddScoped<IGateway<IgnoreParserLinkRequest, ParserLinkResponse>, IgnoreParserLinkGateway>();
            services.AddScoped<IGateway<RenameParserLinkRequest, ParserLinkResponse>, RenameParserLinkGateway>();
            services.AddScoped<IGateway<UnignoreParserLinkRequest, ParserLinkResponse>, UnignoreParserLinkGateway>();
            
            services.Decorate<
                IGateway<AttachParserLinkRequest, ParserLinkResponse>,
                TransactionalGateway<AttachParserLinkRequest, ParserLinkResponse>
            >();
            
            services.Decorate<
                IGateway<ChangeParserLinkRequest, ParserLinkResponse>,
                TransactionalGateway<ChangeParserLinkRequest, ParserLinkResponse>
            >();
            
            services.Decorate<
                IGateway<IgnoreParserLinkRequest, ParserLinkResponse>,
                TransactionalGateway<IgnoreParserLinkRequest, ParserLinkResponse>
            >();
            
            services.Decorate<
                IGateway<RenameParserLinkRequest, ParserLinkResponse>,
                TransactionalGateway<RenameParserLinkRequest, ParserLinkResponse>
            >();
            
            services.Decorate<
                IGateway<UnignoreParserLinkRequest, ParserLinkResponse>,
                TransactionalGateway<UnignoreParserLinkRequest, ParserLinkResponse>
            >();
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