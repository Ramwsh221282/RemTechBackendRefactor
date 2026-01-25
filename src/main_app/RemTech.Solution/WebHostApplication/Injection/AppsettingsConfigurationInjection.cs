using Identity.Infrastructure.Common;
using Identity.WebApi.Options;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Infrastructure.Redis;
using Spares.Infrastructure.Queries.GetSpares;
using Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

namespace WebHostApplication.Injection;

public static class AppsettingsConfigurationInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterConfigurationFromAppsettings()
        {
            services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
            services.AddOptions<EmbeddingsProviderOptions>().BindConfiguration(nameof(EmbeddingsProviderOptions));
            services.AddOptions<FrontendOptions>().BindConfiguration(nameof(FrontendOptions));
            services.AddOptions<NpgSqlOptions>().BindConfiguration(nameof(NpgSqlOptions));
            services.AddOptions<RabbitMqOptions>().BindConfiguration(nameof(RabbitMqOptions));
            services
                .AddOptions<GetVehiclesThresholdConstants>()
                .BindConfiguration(nameof(GetVehiclesThresholdConstants));
            services.AddOptions<GetSparesThresholdConstants>().BindConfiguration(nameof(GetSparesThresholdConstants));
            services.AddOptions<SuperUserCredentialsOptions>().BindConfiguration(nameof(SuperUserCredentialsOptions));
            services.AddOptions<BcryptWorkFactorOptions>().BindConfiguration(nameof(BcryptWorkFactorOptions));
            services.AddOptions<JwtOptions>().BindConfiguration(nameof(JwtOptions));
            services.AddOptions<CachingOptions>().BindConfiguration(nameof(CachingOptions));
        }
    }
}
