using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using Notifications.Core.PendingEmails.Contracts;
using Notifications.Infrastructure.Common;
using Notifications.Infrastructure.Common.Migrations;
using Notifications.Infrastructure.EmailSending;
using Notifications.Infrastructure.Mailers;
using Notifications.Infrastructure.PendingEmails;
using Notifications.Infrastructure.PendingEmails.BackgroundServices;
using Notifications.Infrastructure.RabbitMq.Consumers;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.AesEncryption;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Notifications.WebApi.Extensions;

public static class NotificationsModuleInjection
{
    extension(IServiceCollection services)
    {
        public void AddNotificationsModule(bool isDevelopment)
        {
            services.AddSharedDependencies(isDevelopment);
            services.AddDomainLayer();
            services.AddInfrastructureLayer();
        }
        
        private void AddSharedDependencies(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
                services.AddAesEncryptionOptionsFromAppsettings();
                FrontendOptionsExtensions.AddFromAppsettings(services);
            }
            
            services.AddPostgres();
            services.AddRabbitMq();
            services.AddAesCryptography();
        }
        
        private void AddDomainLayer() =>
            new HandlersRegistrator(services)
                .FromAssemblies([typeof(Mailer).Assembly])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .AlsoAddDomainEventHandlers()
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();

        private void AddInfrastructureLayer()
        {
            services.AddPersistence();
            services.AddCryptography();
            services.AddEmailSender();
            services.AddBackgroundServices();
            services.AddConsumers();
        }

        private void AddBackgroundServices()
        {
            services.AddHostedService<PendingEmailsProcessor>();
        }
        
        private void AddPersistence()
        {
            services.AddMigrations([typeof(NotificationsModuleSchemaMigration).Assembly]);
            services.AddScoped<IMailersRepository, MailersRepository>();
            services.AddScoped<IPendingEmailNotificationsRepository, PendingEmailNotificationsRepository>();
            services.AddScoped<MailersChangeTracker>();
            services.AddScoped<PendingEmailsChangeTracker>();
            services.AddScoped<INotificationsModuleUnitOfWork, NotificationsModuleUnitOfWork>();
        }

        private void AddConsumers()
        {
            services.AddConsumersFromAssemblies([typeof(OnNewAccountCreatedConsumer).Assembly]);
            services.AddAggregatedConsumersBackgroundService();
        }
        
        private void AddEmailSender() =>
            services.AddSingleton<EmailSender>();

        private void AddCryptography() =>
            services.AddSingleton<IMailerCredentialsCryptography, MailerCredentialsCryptography>();
    }
}