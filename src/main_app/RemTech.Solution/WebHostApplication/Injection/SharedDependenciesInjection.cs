using Microsoft.Extensions.DependencyInjection.Extensions;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Redis;
using RemTech.SharedKernel.NN;

namespace WebHostApplication.Injection;

public static class SharedDependenciesInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterSharedDependencies()
        {
            services.RegisterLogging();
            services.AddPostgres();
            services.AddRabbitMq();
            services.RegisterHybridCache();
            RemTech.SharedKernel.Infrastructure.AesEncryption.AesCryptographyExtensions.AddAesCryptography(services);
            services.TryAddSingleton<EmbeddingsProvider>();
        }
    }
}