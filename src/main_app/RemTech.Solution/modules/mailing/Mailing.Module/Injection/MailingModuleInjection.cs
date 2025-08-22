using System.Threading.Channels;
using DbUp;
using DbUp.Engine;
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

    public static void UpDatabase(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(NpgSqlEmailSendersSource).Assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create parsers management database.");
    }
}
