using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Common;

public static class JwtSecretKeyInjection
{
    extension(IServiceCollection services)
    {
        public void AddJwtOptionsFromAppsettings(string sectionName = nameof(JwtOptions))
        {
            services.AddOptions<JwtOptions>().BindConfiguration(sectionName);
        }
    }
}