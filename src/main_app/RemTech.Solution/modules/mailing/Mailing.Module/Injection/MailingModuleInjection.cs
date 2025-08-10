using System.Threading.Channels;
using DbUp;
using DbUp.Engine;
using Mailing.Module.Bus;
using Mailing.Module.Contracts;
using Mailing.Module.Features;
using Mailing.Module.Sources.NpgSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
    }

    public static void MapMailingModuleEndpoints(this WebApplication app)
    {
        RouteGroupBuilder builder = app.MapGroup("api/mailing").RequireCors("FRONTEND");
        CreateMailingSender.Map(builder);
        RemoveMailingSender.Map(builder);
        PingMailingSender.Map(builder);
        ReadMailingSenders.Map(builder);
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
