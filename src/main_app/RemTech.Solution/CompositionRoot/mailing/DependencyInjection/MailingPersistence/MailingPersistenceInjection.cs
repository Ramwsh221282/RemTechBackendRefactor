using CompositionRoot.Shared;
using Mailers.Persistence.NpgSql.MailersModule;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.mailing.DependencyInjection.MailingPersistence;

[DependencyInjectionClass]
internal static class MailingPersistenceInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddMailersPersistence();
    }
}