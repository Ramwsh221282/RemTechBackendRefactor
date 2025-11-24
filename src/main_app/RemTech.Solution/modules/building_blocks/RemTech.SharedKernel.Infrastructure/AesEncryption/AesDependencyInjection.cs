using Microsoft.Extensions.DependencyInjection;

namespace RemTech.SharedKernel.Infrastructure.AesEncryption;

public static class AesDependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddAesCryptography()
        {
            services.AddOptions<AesEncryptionOptions>().BindConfiguration(nameof(AesEncryptionOptions));
            services.AddSingleton<AesCryptography>();
        }
    }
}