using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.AesEncryption;

public static class AesCryptographyExtensions
{
    extension(IServiceCollection services)
    {
        public void AddAesCryptography() =>
            services.TryAddSingleton<AesCryptography>();
    }
}