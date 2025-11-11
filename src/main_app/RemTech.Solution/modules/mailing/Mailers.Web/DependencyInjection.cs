using Mailers.Application;
using Mailers.Application.Configs;
using Mailers.Persistence.NpgSql.MailersModule;
using RemTech.NpgSql.Abstractions;

namespace Mailers.Web;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailersModuleDependencies()
        {
            services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
            services.AddOptions<MailersEncryptOptions>().BindConfiguration(nameof(MailersEncryptOptions));
            services.AddPostgres();
            services.AddMailersApplication();
            services.AddMailersPersistence();
        }
    }

    extension(WebApplication application)
    {
        public void ApplyMailersDbMigration()
        {
            IServiceScope scope = application.Services.CreateScope();
            IDbUpgrader upgrader = scope.ServiceProvider.GetRequiredKeyedService<IDbUpgrader>(nameof(MailersDbUpgrader));
            upgrader.ApplyMigrations();
        }
    }
}