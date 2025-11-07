using System.Threading.Channels;
using Mailing.Moduled.Bus;
using Mailing.Moduled.Cache;
using Mailing.Moduled.Contracts;
using Mailing.Moduled.Features;
using Mailing.Moduled.Public;
using Mailing.Moduled.Sources.NpgSql;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.Moduled.Injection;

public static class MailingModuleInjection
{
    public static void InjectMailingModule(this IServiceCollection services)
    {
        services.AddSingleton<IEmailSendersSource, NpgSqlEmailSendersSource>();
        services.AddSingleton(Channel.CreateUnbounded<MailingBusMessage>());
        services.AddSingleton<MailingBusPublisher>();
        services.AddHostedService<MailingBusReceiver>();
        services.AddHostedService<InitSendersOnStart>();
        services.AddSingleton<HasSenderApi>();
        services.AddSingleton<MailingSendersCache>();
    }
}