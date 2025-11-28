using Identity.Application.Permissions.Contracts;
using Identity.Contracts.AccountPermissions.Contracts;
using Identity.Contracts.Accounts;
using Identity.Contracts.AccountTickets.Contracts;
using Identity.Gateways.AccountPermissions.AttachPermissionToAccount;
using Identity.Gateways.AccountPermissions.DetachPermissionFromAccount;
using Identity.Gateways.AccountPermissions.Shared;
using Identity.Gateways.Accounts.Activate;
using Identity.Gateways.Accounts.AddAccount;
using Identity.Gateways.Accounts.ChangeEmail;
using Identity.Gateways.Accounts.ChangePassword;
using Identity.Gateways.Accounts.RequireActivation;
using Identity.Gateways.Accounts.RequirePasswordReset;
using Identity.Gateways.Accounts.Responses;
using Identity.Gateways.AccountTickets.OnAccountTicketActivationRequired;
using Identity.Gateways.AccountTickets.OnAccountTicketPasswordResetRequired;
using Identity.Gateways.AccountTickets.RabbitMq;
using Identity.Gateways.Common;
using Identity.Gateways.Permissions.AddPermission;
using Identity.Gateways.Permissions.RenamePermission;
using Identity.Gateways.Quartz.OutboxProcessor;
using Identity.Infrastructure;
using Identity.Infrastructure.AccountPermissions;
using Identity.Infrastructure.AccountPermissions.EventListeners.OnRegistered;
using Identity.Infrastructure.AccountPermissions.EventListeners.OnRemoved;
using Identity.Infrastructure.Accounts;
using Identity.Infrastructure.AccountTickets;
using Identity.Infrastructure.ACL;
using Identity.Infrastructure.Outbox;
using Identity.Infrastructure.Permissions;
using Identity.Infrastructure.Permissions.EventListeners.OnCreate;
using Identity.Infrastructure.Permissions.EventListeners.OnRename;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.Quartz;
using RemTech.SharedKernel.Infrastructure.RabbitMq.Shared;

namespace Identity.CompositionRoot;

public static class IdentityModuleInjection
{
    extension(IServiceCollection services)
    {
        public void AddIdentityModule()
        {
            services.RegisterAccountContextDependencies();
            services.RegisterAccountTicketsContextDependencies();
            services.RegisterPermissionDependencies();
            services.RegisterAccountPermissionsContext();
            services.RegisterOutbox();
        }
        
        private void RegisterAccountContextDependencies()
        {
            services.AddScoped<IAccountsStorage, NpgSqlAccountsStorage>();
            services.AddSingleton<IAccountCryptography, AesAccountCryptography>();
            services.AddTransient<IDbUpgrader, IdentityModuleDbUpgrader>();
            
            services.AddScoped<IGateway<AddAccountRequest, AccountResponse>, AddAccountGateway>();
            services.AddScoped<IGateway<ActivateAccountRequest, AccountResponse>, ActivateAccountGateway>();
            services.AddScoped<IGateway<ChangeAccountEmailRequest, AccountResponse>,  ChangeAccountEmailGateway>();
            services.AddScoped<IGateway<ChangeAccountPasswordRequest, AccountResponse>,  ChangeAccountPasswordGateway>();
            services.AddScoped<IGateway<RequireActivationRequest, RequireActivationResponse>, RequireActivationGateway>();
            services.AddScoped<IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse>, RequirePasswordResetGateway>();
            
            services.Decorate<
                IGateway<ChangeAccountEmailRequest, AccountResponse>, 
                TransactionalGateway<ChangeAccountEmailRequest, AccountResponse>>();
            
            services.Decorate<
                IGateway<ChangeAccountPasswordRequest, AccountResponse>, 
                TransactionalGateway<ChangeAccountPasswordRequest, AccountResponse>>();
            
            services.Decorate<
                IGateway<RequireActivationRequest, RequireActivationResponse>,
                TransactionalGateway<RequireActivationRequest, RequireActivationResponse>>();
        }

        private void RegisterAccountTicketsContextDependencies()
        {
            services.AddScoped<IAccountTicketsStorage, NpgSqlAccountTicketsStorage>();
            
            services.AddScoped<
                IOnAccountActivationRequiredListener, 
                AddAccountTicketOnAccountActivationRequested>();

            services.AddScoped<
                IOnAccountPasswordResetRequiredListener,
                AddAccountTicketOnAccountPasswordResetRequired
            >();
            
            services.Decorate<
                IOnAccountActivationRequiredListener, 
                LoggingAddAccountTicketOnAccountActivationRequested>();

            services.Decorate<
                IOnAccountPasswordResetRequiredListener,
                LoggingAddAccountTicketOnAccountPasswordResetRequired>();
        }

        private void RegisterPermissionDependencies()
        {
            services.AddScoped<IPermissionsStorage, NpgSqlPermissionsStorage>();
            services.AddScoped<NpgSqlPermissionsStorage>();
            services.AddScoped<LoggingPermissionCreatedEventListener>();
            services.AddScoped<NpgSqlPermissionCreatedEventListener>();
            services.AddScoped<LoggingPermissionRenamedEventListener>();
            services.AddScoped<NpgSqlPermissionRenamedEventListener>();
            
            services.AddScoped<IGateway<AddPermissionRequest, AddPermissionResponse>, AddPermissionGateway>();
            services.AddScoped<IGateway<RenamePermissionRequest, RenamePermissionResponse>, RenamePermissionGateway>();
            
            services.Decorate<
                IGateway<RenamePermissionRequest, RenamePermissionResponse>,
                TransactionalGateway<RenamePermissionRequest, RenamePermissionResponse>
            >();
        }
        
        private void RegisterOutbox()
        {
            services.AddTransient<RabbitMqPublisherLogger>();
            services.AddScoped<IdentityOutboxMessagesStore>();
            services.AddTransient<RabbitMqPublishingMethod>();
            services.AddTransient<ICronScheduleJob, IdentityOutboxProcessor>();
            services.AddScoped<IOnAccountTicketCreatedEventListener, OutboxOnAccountTicketCreatedEventListener>();
            services.AddTransient<IRabbitMqOutboxMessagePublisher, OnAccountActivationRequiredRabbitMqPublisher>();
            services.AddTransient<IRabbitMqOutboxMessagePublisher, OnAccountPasswordResetRequiredRabbitMqPublisher>();
            services.AddTransient<RabbitMqOutboxMessagePublishersRegistry>(sp =>
            {
                RabbitMqOutboxMessagePublishersRegistry registry = new();
                IEnumerable<IRabbitMqOutboxMessagePublisher> publishers =
                    sp.GetServices<IRabbitMqOutboxMessagePublisher>();
                registry.AddPublishers(publishers);
                return registry;
            });
        }

        private void RegisterAccountPermissionsContext()
        {
            services.AddScoped<
                    IGateway<AttachPermissionToAccountRequest, AccountPermissionResponse>,
                    AttachPermissionToAccountGateway>();
            
            services.AddScoped<
                IGateway<DetachPermissionFromAccountRequest, AccountPermissionResponse>,
                DetachPermissionFromAccountGateway>();

            services.AddScoped<QueryAccountAndPermission>();
            services.AddScoped<NpgSqlOnAccountPermissionRegisteredEventListener>();
            services.AddScoped<NpgSqlOnAccountPermissionRemovedEventListener>();
            services.AddScoped<LoggingOnAccountPermissionRemovedEventListener>();
            services.AddScoped<LoggingOnRegisteredAccountPermissionEventListener>();
            services.AddScoped<IAccountPermissionsStorage, NpgSqlAccountPermissionsStorage>();
            services.AddScoped<NpgSqlAccountPermissionsStorage>();
            
            services.Decorate<
                IGateway<AttachPermissionToAccountRequest, AccountPermissionResponse>,
                TransactionalGateway<AttachPermissionToAccountRequest, AccountPermissionResponse>>();
            
            services.Decorate<
                IGateway<DetachPermissionFromAccountRequest, AccountPermissionResponse>,
                TransactionalGateway<DetachPermissionFromAccountRequest, AccountPermissionResponse>>();
        }
    }
}