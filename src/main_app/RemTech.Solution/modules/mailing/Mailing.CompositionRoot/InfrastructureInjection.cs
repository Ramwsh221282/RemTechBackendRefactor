using Mailing.Core.Inbox.Protocols;
using Mailing.Core.Mailers.Protocols;
using Mailing.Infrastructure.AesEncryption;
using Mailing.Infrastructure.EventListeners;
using Mailing.Infrastructure.InboxMessageProcessing;
using Mailing.Infrastructure.InboxMessageProcessing.Protocols;
using Mailing.Infrastructure.MimeMessage;
using Mailing.Infrastructure.NpgSql;
using Mailing.Infrastructure.NpgSql.Inbox;
using Mailing.Infrastructure.NpgSql.Mailers;
using Mailing.Infrastructure.NpgSql.Seeder;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.Quartz;

namespace Mailing.CompositionRoot;

public static class InfrastructureInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailingInfrastructureDependencies()
        {
            services.AddMailerConfigCryptographicProtocol();
            services.AddMailersPersistence();
            services.AddMimeMessageSmtpSendingProtocol();
            services.AddMailerInboxProcessingDependencies();
            services.AddEventListeners();
        }
        
        private void AddMailerConfigCryptographicProtocol()
        {
            services.AddSingleton<EncryptMailerSmtpPasswordProtocol, MailerCryptoProtocol>();
            services.AddSingleton<DecryptMailerSmtpPasswordProtocol, MailerCryptoProtocol>();
        }

        private void AddMailersPersistence()
        {
            services.AddScoped<PersistMailerProtocol, NpgSqlPersistMailerProtocol>();
            services.AddScoped<SaveMailerProtocol, NpgSqlSaveMailerProtocol>();
            services.AddScoped<SaveInboxMessageProtocol, NpgSqlSaveInboxMessageProtocol>();
            services.AddScoped<GetMailerProtocol, NpgSqlGetMailerProtocol>();
            services.AddScoped<GetPendingInboxMessagesProtocol, NpgSqlBatchProcessingInboxMessagesProtocol>();
            services.AddScoped<RemoveManyInboxMessagesProtocol, NpgSqlRemoveManyInboxMessagesProtocol>();
            services.AddScoped<NpgSqlHasInboxMessagesProtocol>();
            services.AddScoped<EnsureMailerEmailUniqueProtocol, NpgSqlEnsureEmailUniqueProtocol>();
            services.AddTransient<IDbUpgrader, MailingModuleDbUpgrader>();
            services.AddTransient<InboxMessagesSeeder>();
        }

        private void AddMailerInboxProcessingDependencies()
        {
            services.AddTransient<ICronScheduleJob, InboxMessagesProcessor>();
            services.AddTransient<InboxMessagesProcessorProcedureDependencies>();
            services.AddTransient<InboxMessagesProcessorProtocol, InboxMessagesProcessorProcedure>();
        }

        private void AddEventListeners()
        {
            services.AddHostedService<AccountActivationTicketRabbitMqListener>();
        }
        
        private void AddMimeMessageSmtpSendingProtocol()
        {
            services.AddScoped<MessageDeliveryProtocol, MimeMessageDeliveringProtocol>();
        }
    }
}