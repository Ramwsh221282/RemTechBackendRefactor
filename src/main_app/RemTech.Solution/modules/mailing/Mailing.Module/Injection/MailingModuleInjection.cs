using System.Threading.Channels;
using Mailing.Module.Bus;
using Mailing.Module.Cache;
using Mailing.Module.Contracts;
using Mailing.Module.Features;
using Mailing.Module.Public;
using Mailing.Module.Sources.NpgSql;
using Microsoft.Extensions.DependencyInjection;

namespace Mailing.Module.Injection;

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
