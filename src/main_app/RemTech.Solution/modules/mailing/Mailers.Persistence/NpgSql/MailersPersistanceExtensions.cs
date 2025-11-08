using Microsoft.Extensions.DependencyInjection;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Persistence.NpgSql;

public static class MailersPersistanceExtensions
{
    extension(IServiceCollection services)
    {
        public void AddMailersPersistance()
        {
            services.AddKeyedSingleton<IDbUpgrader, MailersDbUpgrader>(nameof(MailersDbUpgrader));
        }
    }
}