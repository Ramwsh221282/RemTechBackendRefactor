using Mailers.Core;
using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql;

public static class MailersPersistenceExtensions
{
    extension(IServiceCollection services)
    {
        public void AddMailersPersistence()
        {
            services.AddKeyedSingleton<IDbUpgrader, MailersDbUpgrader>(nameof(MailersDbUpgrader));
            services.RegisterAsyncFunctions<TableMailer>();
            services.RegisterFunctions<TableMailer>();
        }
    }
}