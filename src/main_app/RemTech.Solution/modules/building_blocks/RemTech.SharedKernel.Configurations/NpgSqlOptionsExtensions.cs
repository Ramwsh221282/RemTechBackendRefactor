using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Configurations;

public static class NpgSqlOptionsExtensions
{
    extension(IServiceCollection services)
    {
        public void AddNpgSqlOptionsFromAppsettings(string sectionName = nameof(NpgSqlOptions))
        {
            services.AddOptions<NpgSqlOptions>().BindConfiguration(sectionName);
        }
    }
}