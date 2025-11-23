using Mailing.Core.Inbox.Protocols;
using Mailing.Core.Mailers.Protocols;
using Mailing.Infrastructure.AesEncryption;
using Mailing.Infrastructure.MimeMessage;
using Mailing.Infrastructure.NpgSql;
using Mailing.Infrastructure.NpgSql.Inbox;
using Mailing.Infrastructure.NpgSql.Mailers;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

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
        }
        
        private void AddMailerConfigCryptographicProtocol()
        {
            services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
            services.AddSingleton<EncryptMailerSmtpPasswordProtocol, MailerConfigCryptoProtocol>();
            services.AddSingleton<DecryptMailerSmtpPasswordProtocol, MailerConfigCryptoProtocol>();
        }

        private void AddMailersPersistence()
        {
            services.AddScoped<PersistMailerProtocol, NpgSqlPersistMailerProtocol>();
            services.AddScoped<SaveMailerProtocol, NpgSqlSaveMailerProtocol>();
            services.AddScoped<SaveInboxMessageProtocol, NpgSqlSaveInboxMessageProtocol>();
            services.AddScoped<GetMailerProtocol, NpgSqlGetMailerProtocol>();
            services.AddTransient<IDbUpgrader, MailingModuleDbUpgrader>();
        }

        private void AddMimeMessageSmtpSendingProtocol()
        {
            services.AddScoped<MessageDeliveryProtocol, MimeMessageDeliveringProtocol>();
        }
    }
}