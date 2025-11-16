using CompositionRoot.Shared;
using Mailers.Core.MailersModule;
using Mailers.Persistence.NpgSql.MailedMessagesModule;
using Mailers.Persistence.NpgSql.MailersModule;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace CompositionRoot.mailing.DependencyInjection.MailingPersistence;

[DependencyInjectionClass]
public static class MailingPersistenceInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTransient<IDbUpgrader, MailersDbUpgrader>();
        services.AddScoped<InsertMailer>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailers.InsertMailer(session);
        });
        services.AddScoped<DeleteMailer>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailers.DeleteMailer(session);
        });
        services.AddScoped<UpdateMailer>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailers.UpdateMailer(session);
        });
        services.AddScoped<GetMailer>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailers.GetMailer(session);
        });
        services.AddScoped<GetManyMailers>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailers.GetMany(session);
        });
        services.AddScoped<HasUniqueMailerEmail>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailers.HasUniqueMailerEmail(session);
        });
        services.AddScoped<InsertMailerSending>(sp =>
        {
            NpgSqlSession session = sp.ExtractSessionFromIoC();
            return NpgSqlMailerSendings.InsertMailerSending(session);
        });
        services.AddScoped<NpgSqlMailersCommands>();
    }
}